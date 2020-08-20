using System;
using System.Linq;
using HarmonyLib;
using SDG.Unturned;

namespace OpenMod.Unturned.Module.Shared
{
    // this patch replaces RocketMod.Unturned dependencies with OpenMod.Unturned
    // so third party modules can work with the OpenMod RocketMod Bridge (see #160)
    [HarmonyPatch(typeof(SDG.Framework.Modules.Module))]
    [HarmonyPatch("load")]
    public static class ModuleloadPatch
    {
        [HarmonyPrefix]
        public static void load(SDG.Framework.Modules.Module __instance)
        {
            var dependency = __instance.config.Dependencies.FirstOrDefault(d => d.Name.Equals("RocketMod.Unturned", StringComparison.OrdinalIgnoreCase));
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