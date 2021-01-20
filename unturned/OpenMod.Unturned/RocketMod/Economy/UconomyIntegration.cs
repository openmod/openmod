using System.IO;
using System.Reflection;

namespace OpenMod.Unturned.RocketMod.Economy
{
    public static class UconomyIntegration
    {
        /// <summary>
        /// Returns true if Uconomy is installed.
        /// </summary>
        public static bool IsUconomyInstalled()
        {
            return GetUconomyFile() != null;
        }

        /// <summary>
        /// Gets the path to the Uconomy assembly. Will return null if not found.
        /// </summary>
        public static string GetUconomyFile()
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
                    continue;
                }
            }

            return null;
        }
    }
}