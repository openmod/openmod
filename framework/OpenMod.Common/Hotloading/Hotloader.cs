using dnlib.DotNet;
using OpenMod.Common.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OpenMod.Common.Hotloading
{
    /// <summary>
    /// Adds support for hotloading assemblies.
    /// Use <see cref="LoadAssembly(byte[])"/> instead of <see cref="Assembly.Load(byte[])"/>.
    /// </summary>
    public static class Hotloader
    {
        private static readonly Dictionary<string, Assembly> s_Assemblies;
        private static readonly bool s_IsMono;
        private static readonly ModuleContext s_ModuleContext;

        /// <summary>
        /// Defines if hotloading is enabled.
        /// </summary>
        public static bool Enabled { get; set; }

        static Hotloader()
        {
            s_Assemblies = new Dictionary<string, Assembly>();
            s_IsMono = Type.GetType("Mono.Runtime") is not null;
            s_ModuleContext = ModuleDef.CreateModuleContext(); // should be singleton
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        }

        private static Assembly? OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return GetAssembly(args.Name);
        }

        /// <summary>
        /// Hotloads an assembly. Redirects to <see cref="Assembly.Load(byte[])"/> if <see cref="Enabled"/> is set to false.
        /// </summary>
        /// <param name="assemblyData">The assembly to hotload.</param>
        /// <returns>The loaded assembly.</returns>
        public static Assembly LoadAssembly(byte[] assemblyData)
        {
            return LoadAssembly(assemblyData, assemblySymbols: null);
        }

        /// <summary>
        /// Hotloads an assembly. Redirects to <see cref="Assembly.Load(byte[], byte[])"/> if <see cref="Enabled"/> is set to false.
        /// </summary>
        /// <param name="assemblyData">The assembly to hotload.</param>
        /// <param name="assemblySymbols">A byte array that contains the raw bytes representing the symbols for the assembly.</param>
        /// <returns>The loaded assembly.</returns>
        public static Assembly LoadAssembly(byte[] assemblyData, byte[]? assemblySymbols)
        {
            if (!Enabled)
            {
                return Assembly.Load(assemblyData, assemblySymbols);
            }

            using var module = ModuleDefMD.Load(assemblyData, s_ModuleContext);

            var isStrongNamed = module.Assembly.PublicKey is not null and { IsNullOrEmpty: false };

            if (!s_IsMono && isStrongNamed)
            {
                // Don't hotload strong-named assemblies unless mono
                // Will cause FileLoadException's if not mono
                return Assembly.Load(assemblyData, assemblySymbols);
            }

            var realFullname = module.Assembly.FullName;

            s_Assemblies.Remove(realFullname);

            var guid = Guid.NewGuid().ToString("N").Substring(0, 6);
            var name = $"{module.Assembly.Name}-{guid}";

            module.Assembly.Name = name;
            module.Assembly.PublicKey = null;
            module.Assembly.HasPublicKey = false;

            using var output = new MemoryStream();
            module.Write(output);

            var newAssemblyData = output.ToArray();
            var assembly = Assembly.Load(newAssemblyData, assemblySymbols);
            s_Assemblies.Add(realFullname, assembly);
            return assembly;
        }

        /// <summary>
        /// Removes an assembly from the hotloader cache.
        /// </summary>
        /// <param name="assembly">The assembly to remove.</param>
        public static void Remove(Assembly assembly)
        {
            foreach (var key in s_Assemblies
                .Where(kv => kv.Value == assembly)
                .Select(x => x.Key)
                .ToArray())
            {
                s_Assemblies.Remove(key);
            }
        }

        /// <summary>
        /// Resolves a hotloaded assembly. Hotloaded assemblies have an auto generated assembly name.
        /// </summary>
        /// <param name="fullname">The assembly name to resolve.</param>
        /// <returns><b>The hotloaded assembly</b> if found; otherwise, <b>null</b>.</returns>
        public static Assembly? GetAssembly(string fullname)
        {
            if (s_Assemblies.TryGetValue(fullname, out var assembly))
            {
                return assembly;
            }

            var name = ReflectionExtensions.GetVersionIndependentName(fullname);

            foreach (var kv in s_Assemblies)
            {
                if (ReflectionExtensions.GetVersionIndependentName(kv.Key).Equals(name))
                {
                    return kv.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets all hotloaded assemblies.
        /// </summary>
        /// <returns>The hotloaded assemblies.</returns>
        public static IReadOnlyCollection<Assembly> GetHotloadedAssemblies()
        {
            return s_Assemblies.Values;
        }

        /// <summary>
        /// Gets the real assembly name of an hotloaded assembly. Hotloaded assemblies have an auto generated assembly name.
        /// </summary>
        /// <param name="assembly">The assembly to get the real name of.</param>
        /// <returns><b>The real assembly name</b> of the hotloaded assembly. If the given assembly was not hotloaded, it will return <b>the assembly's name</b>.</returns>
        public static AssemblyName GetRealAssemblyName(Assembly assembly)
        {
            foreach (var kv in s_Assemblies)
            {
                if (kv.Value == assembly)
                {
                    return new AssemblyName(kv.Key);
                }
            }

            return assembly.GetName();
        }
    }
}
