using HarmonyLib;

namespace OpenMod.Unturned.Module.Shared.Patches
{
    // this patch replaces RocketMod.Unturned dependencies with OpenMod.Unturned
    // so third party modules can work with the OpenMod RocketMod Bridge (see #160)
    [HarmonyPatch(typeof(SDG.Framework.Modules.Module))]
    [HarmonyPatch("load")]
    public static class ModuleLoadPatch
    {
        [HarmonyPrefix]
        public static void load(SDG.Framework.Modules.Module __instance)
        {
            ModuleHelper.FixRocketModDependencies(__instance.config);
        }
    }
}