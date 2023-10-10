using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using SDG.Framework.Modules;

namespace OpenMod.Unturned.Module.Bootstrapper
{
    /// <summary>
    /// Bootstrapper of main or dev OpenMod module.
    /// </summary>
    [UsedImplicitly]
    public class BootstrapperModule : IModuleNexus
    {
        private IModuleNexus? m_BootstrappedModule;
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
            var bootstrapperAssemblyFilePath = string.Empty;
            var bootstrapperAssembly = typeof(BootstrapperModule).Assembly;

            foreach (var module in ModuleHook.modules)
            {
                if (module.assemblies is { Length: > 0 } && module.assemblies[0] == bootstrapperAssembly)
                {
                    openModModuleDirectory = module.config.DirectoryPath;
                    bootstrapperAssemblyFilePath = module.config.DirectoryPath + module.config.Assemblies[0].Path;
                    break;
                }
            }

            if (string.IsNullOrEmpty(openModModuleDirectory))
            {
                throw new Exception("Failed to get OpenMod module directory");
            }

            m_LoadedAssemblies = new();
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            Assembly? moduleAssembly = null;

            foreach (var assemblyFilePath in Directory.GetFiles(openModModuleDirectory, "*.dll", SearchOption.TopDirectoryOnly))
            {
                // ignore bootstrapper assembly file
                if (assemblyFilePath == bootstrapperAssemblyFilePath)
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

            Type[] types;
            try
            {
                types = moduleAssembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.Where(d => d != null).ToArray();
            }

            var moduleType = types.SingleOrDefault(d => d.Name.Equals("OpenModUnturnedModule"));
            if (moduleType == null)
            {
                throw new Exception($"Failed to find OpenModUnturnedModule class in {moduleAssembly}!");
            }

            m_BootstrappedModule = (IModuleNexus)Activator.CreateInstance(moduleType);
            m_BootstrappedModule.initialize();
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

        private Assembly? CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
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
            m_BootstrappedModule?.shutdown();

            m_LoadedAssemblies?.Clear();
            m_LoadedAssemblies = null;
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;

            Instance = null;
        }
    }
}