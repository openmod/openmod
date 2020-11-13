using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using OpenMod.Core.Helpers;

namespace OpenMod.Core.Hotloading
{
    public static class AssemblyHotloader
    {
        private static readonly Dictionary<string, Assembly> s_Assemblies;

        static AssemblyHotloader()
        {
            s_Assemblies = new Dictionary<string, Assembly>();
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        }

        private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            Assembly match = null;
            var name = ReflectionExtensions.GetVersionIndependentName(args.Name);

            foreach (var kv in s_Assemblies)
            {
                if (kv.Key.Equals(args.Name))
                {
                    return kv.Value;
                }

                if (ReflectionExtensions.GetVersionIndependentName(kv.Key).Equals(name))
                {
                    match = kv.Value;
                }
            }

            return match;
        }

        public static Assembly LoadAssembly(byte[] assemblyData)
        {
            using var input = new MemoryStream(assemblyData, writable: false);
            using var output = new MemoryStream();

            var module = ModuleDefinition.ReadModule(input);
            var realFullname = module.Assembly.FullName;

            var name = Guid.NewGuid().ToString().Replace("-", "");
            module.Assembly.Name = new AssemblyNameDefinition(name, new Version(0, 0));
            module.Write(output);
            output.Seek(offset: 0, SeekOrigin.Begin);

            var newAssemblyData = output.ToArray();
            var assembly = Assembly.Load(newAssemblyData);
            s_Assemblies.Add(realFullname, assembly);
            return assembly;
        }

        public static void Clear()
        {
            s_Assemblies.Clear();
        }

        public static void Remove(Assembly assembly)
        {
            foreach (var kv in s_Assemblies.Where(kv => kv.Value == assembly))
            {
                s_Assemblies.Remove(kv.Key);
            }
        }
    }
}