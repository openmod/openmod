using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using dnlib.DotNet;
using OpenMod.Core.Helpers;

namespace OpenMod.Core.Hotloading
{
    public static class Hotloader
    {
        private static readonly Dictionary<string, Assembly> s_Assemblies;

        static Hotloader()
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
            return Assembly.Load(assemblyData);
            /*
            using var input = new MemoryStream(assemblyData, writable: false);
            using var output = new MemoryStream();

            var modCtx = ModuleDef.CreateModuleContext();
            var module = ModuleDefMD.Load(input, modCtx);

            var realFullname = module.Assembly.FullName;

            if (s_Assemblies.ContainsKey(realFullname))
            {
                s_Assemblies.Remove(realFullname);
            }

            var guid = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 6);
            var name = $"{module.Assembly.Name}-{guid}";

            module.Assembly.Name = name; 
            module.Assembly.PublicKey = null;
            module.Assembly.HasPublicKey = false;

            module.Write(output);
            output.Seek(offset: 0, SeekOrigin.Begin);

            var newAssemblyData = output.ToArray();
            var assembly = Assembly.Load(newAssemblyData);
            s_Assemblies.Add(realFullname, assembly);
            return assembly;
            */
        }

        public static void Remove(Assembly assembly)
        {
            foreach (var kv in s_Assemblies.Where(kv => kv.Value == assembly))
            {
                s_Assemblies.Remove(kv.Key);
            }
        }

        public static Assembly GetAssembly(string fullname)
        {
            return s_Assemblies[fullname];
        }

        public static IReadOnlyCollection<Assembly> GetHotloadedAssemblies()
        {
            return s_Assemblies.Values;
        }

        public static AssemblyName GetRealName(Assembly assembly)
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