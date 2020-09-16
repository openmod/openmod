using System;
using System.Linq;
using SDG.Framework.Modules;
using SDG.Unturned;

namespace Rocket.Unturned.Patches
{
    internal static class ModuleHelper
    {
        public static void RedirectRocketModDependencies(ModuleConfig config)
        {
            var dependency = config.Dependencies.FirstOrDefault(d => d.Name.Equals("Rocket.Unturned", StringComparison.OrdinalIgnoreCase));
            if (dependency == null)
            {
                return;
            }

            dependency.Name = "OpenMod.Unturned";
            dependency.Version = "0.0.0.0";
            dependency.Version_Internal = Parser.getUInt32FromIP(dependency.Version);
        }
    }
}