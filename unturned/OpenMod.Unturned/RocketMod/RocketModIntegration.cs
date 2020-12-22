using System;
using System.IO;
using System.Linq;
using SDG.Unturned;

namespace OpenMod.Unturned.RocketMod
{
    public static class RocketModIntegration
    {
        private static bool s_Installed;

        public static bool IsRocketModInstalled()
        {
            return File.Exists(Path.Combine(ReadWrite.PATH, "Modules", "Rocket.Unturned", "Rocket.Unturned.module"));
        }

        public static bool IsRocketModLoaded()
        {
            return AppDomain.CurrentDomain.GetAssemblies().Any(d => d.GetName().Name.Equals("Rocket.Unturned"));
        }

        public static void Install()
        {
            if (s_Installed)
            {
                return;
            }

            s_Installed = true;
        }

        public static void Uninstall()
        {
            if (!s_Installed)
            {
                return;
            }

            s_Installed = false;
        }
    }
}