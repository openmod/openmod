using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OpenMod.Unturned.RocketMod.Economy
{
    public static class UconomyIntegration
    {
        internal const string HarmonyId = "com.get-openmod.unturned.module.rocketmod.uconomy";

        /// <summary>
        /// Returns true if Uconomy is installed.
        /// </summary>
        public static bool IsUconomyInstalled()
        {
            return GetUconomyFile() != null;
        }

        /// <summary>
        /// Returns true if the Uconomy assembly is loaded.
        /// </summary>
        /// <returns></returns>
        public static bool IsUconomyLoaded()
        {
            return GetUconomyAssembly() != null;
        }

        /// <summary>
        /// Gets the Uconomy assembly if loaded or null.
        /// </summary>
        public static Assembly? GetUconomyAssembly()
        {
            return AppDomain.CurrentDomain.GetAssemblies().LastOrDefault(d => d.GetName().Name.Equals("Uconomy"));
        }

        /// <summary>
        /// Gets the path to the Uconomy assembly. Will return null if not found.
        /// </summary>
        public static string? GetUconomyFile()
        {
            var pluginsFolder = Path.Combine(RocketModIntegration.GetRocketFolder(), "Plugins");

            if (!Directory.Exists(pluginsFolder))
            {
                return null;
            }

            foreach (var file in Directory.GetFiles(pluginsFolder, "*.dll", SearchOption.AllDirectories))
            {
                try
                {
                    var assemblyName = AssemblyName.GetAssemblyName(file);
                    if (assemblyName.Name.Equals("Uconomy"))
                    {
                        return file;
                    }
                }
                catch
                {
                    // ignored
                }
            }

            return null;
        }
    }
}