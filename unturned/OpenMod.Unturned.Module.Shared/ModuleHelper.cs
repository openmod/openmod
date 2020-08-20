using System;
using System.Linq;
using SDG.Framework.Modules;
using SDG.Unturned;

namespace OpenMod.Unturned.Module.Shared
{
    internal static class ModuleHelper
    {
        public static void FixRocketModDependencies(ModuleConfig config)
        {
            var dependency = config.Dependencies.FirstOrDefault(d => d.Name.Equals("RocketMod.Unturned", StringComparison.OrdinalIgnoreCase));
            if (dependency == null)
            {
                return;
            }

            dependency.Name = "OpenMod.Unturned";
            dependency.Version = typeof(OpenModSharedUnturnedModule).Assembly.GetName().Version.ToString();
            dependency.Version_Internal = Parser.getUInt32FromIP(dependency.Version);
        }
    }
}