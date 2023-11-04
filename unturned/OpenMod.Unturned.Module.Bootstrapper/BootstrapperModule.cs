using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using SDG.Framework.Modules;
using SDG.Unturned;
using UnityEngine;

namespace OpenMod.Unturned.Module.Bootstrapper
{
    /// <summary>
    /// Bootstrapper of main or dev OpenMod module.
    /// </summary>
    [UsedImplicitly]
    public class BootstrapperModule : IModuleNexus
    {
        private IModuleNexus? m_OpenModUnturndModule;
        private ConcurrentDictionary<string, Assembly>? m_LoadedAssemblies;

        /// <summary>
        /// Instance of bootstrapper.
        /// </summary>
        /// <remarks>
        /// Note, this instance is used in hard-reload via reflection.
        /// </remarks>
        public static BootstrapperModule? Instance { get; private set; }

        public void initialize()
        {
            Instance = this;

            var openModModuleDirectory = string.Empty;
            var bootstrapperAssemblyFileName = string.Empty;
            var bootstrapperAssembly = typeof(BootstrapperModule).Assembly;

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var module in ModuleHook.modules)
            {
                // ReSharper disable once InvertIf
                if (module.assemblies is { Length: > 0 } && module.assemblies[0] == bootstrapperAssembly)
                {
                    openModModuleDirectory = Path.GetFullPath(module.config.DirectoryPath);
                    bootstrapperAssemblyFileName = Path.GetFileName(module.config.Assemblies[0].Path);
                    break;
                }
            }

            if (string.IsNullOrEmpty(openModModuleDirectory))
            {
                throw new Exception("Failed to get OpenMod module directory");
            }

            m_LoadedAssemblies = new ConcurrentDictionary<string, Assembly>();
            ModuleHook.PreVanillaAssemblyResolvePostRedirects += UnturnedPreVanillaAssemblyResolve;

            Assembly? moduleAssembly = null;

            foreach (var assemblyFilePath in Directory.GetFiles(openModModuleDirectory, "*.dll", SearchOption.TopDirectoryOnly))
            {
                //Workaround
                //Unturned_Server/Modules\OM\OpenMod.Unturned.Module.Bootstrapper.dll -> assemblyFilePath
                //Unturned_Server/Modules\OM/OpenMod.Unturned.Module.Bootstrapper.dll -> Old BootstrapperAssemblyPath
                if (Path.GetFileName(assemblyFilePath).Equals(bootstrapperAssemblyFileName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var symbolsFilePath = Path.ChangeExtension(assemblyFilePath, "pdb");
                var symbols = File.Exists(symbolsFilePath) ? File.ReadAllBytes(symbolsFilePath) : null;

                var assembly = Assembly.Load(File.ReadAllBytes(assemblyFilePath), symbols);
                m_LoadedAssemblies.TryAdd(assembly.GetName().Name, assembly);

                var fileName = Path.GetFileName(assemblyFilePath);
                if (fileName.Equals("OpenMod.Unturned.Module.dll")
                    || fileName.Equals("OpenMod.Unturned.Module.Dev.dll"))
                {
                    moduleAssembly = assembly;
                }
            }

            if (moduleAssembly == null)
            {
                throw new Exception("Failed to find OpenMod module assembly!");
            }

            AddRocketModResolveAssemblies();

            IEnumerable<Type> types;
            try
            {
                types = moduleAssembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                Console.WriteLine($"OpenMod Bootstrap Module fail to obtain assembly types: {ex}");
                foreach (var loaderException in ex.LoaderExceptions)
                {
                    Console.WriteLine($"Loader exception: {loaderException}");
                }

                Console.WriteLine("Trying to load bootstrap anyways...");
                types = ex.Types.Where(d => d != null);
            }

            var moduleType = types.SingleOrDefault(d => d.Name.Equals("OpenModUnturnedModule")) ?? throw new Exception($"Failed to find OpenModUnturnedModule class in {moduleAssembly}!");
            m_OpenModUnturndModule = (IModuleNexus)Activator.CreateInstance(moduleType);
            m_OpenModUnturndModule.initialize();

            PatchUnturnedVanillaAssemblyResolve();
        }

        /// <summary>
        /// To prevent unnecessary errors and warning about assemblies resolving
        /// We will make Unturned Resolve Assemblies as last resource
        /// With this patch other components like hotloader wil try to resolve assemblies before unturned
        /// </summary>
        private void PatchUnturnedVanillaAssemblyResolve()
        {
            var assemblyResolveMethod = typeof(ModuleHook).GetMethod("handleAssemblyResolve", BindingFlags.NonPublic | BindingFlags.Instance);
            if (assemblyResolveMethod == null)
            {
                Console.WriteLine($"Couldn't find OnAssemblyResolve method for {nameof(ModuleHook)}!");
                return;
            }

            //using
            var provider = typeof(Provider).GetField("steam", BindingFlags.NonPublic | BindingFlags.Static)
                ?.GetValue(null) as MonoBehaviour;
            if (provider == null)
            {
                Console.WriteLine("Couldn't find Provider instance!");
                return;
            }

            var moduleHook = provider.GetComponent<ModuleHook>();
            if (moduleHook == null)
            {
                Console.WriteLine("Couldn't get ModuleHook instance from Provider!");
                return;
            }

            var vanillaDelegate = (ResolveEventHandler)assemblyResolveMethod.CreateDelegate(typeof(ResolveEventHandler), moduleHook);

            AppDomain.CurrentDomain.AssemblyResolve -= vanillaDelegate;
            AppDomain.CurrentDomain.AssemblyResolve += vanillaDelegate;
        }

        /// <summary>
        /// Adds LDM assemblies to our assembly resolver to fix assembly resolve issue due to using different version
        /// </summary>
        private void AddRocketModResolveAssemblies()
        {
            foreach (var module in ModuleHook.modules)
            {
                if (!module.config.Name.Equals("Rocket.Unturned", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                foreach (var moduleAssembly in module.config.Assemblies)
                {
                    var path = module.config.DirectoryPath + moduleAssembly.Path;

                    // let Unturned get the assembly
                    var assembly = ModuleHook.resolveAssemblyPath(path);

                    if (assembly == null)
                    {
                        // should not return null
                        return;
                    }

                    m_LoadedAssemblies?.TryAdd(assembly.GetName().Name, assembly);
                }

                return;
            }
        }

        private Assembly? UnturnedPreVanillaAssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (m_LoadedAssemblies == null)
            {
                return null;
            }

            if (m_LoadedAssemblies.TryGetValue(args.Name, out var assembly))
            {
                return assembly;
            }

            var assemblyName = new AssemblyName(args.Name).Name;
            if (m_LoadedAssemblies.TryGetValue(assemblyName, out assembly))
            {
                m_LoadedAssemblies.TryAdd(args.Name, assembly);
                return assembly;
            }

            return null;
        }

        public void shutdown()
        {
            m_OpenModUnturndModule?.shutdown();

            m_LoadedAssemblies?.Clear();
            m_LoadedAssemblies = null;
            ModuleHook.PreVanillaAssemblyResolvePostRedirects -= UnturnedPreVanillaAssemblyResolve;

            Instance = null;
        }
    }
}