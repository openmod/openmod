using HarmonyLib;
using SDG.Framework.Modules;

namespace OpenMod.Unturned.Module.Shared.Patches
{
    [HarmonyPatch(typeof(SDG.Framework.Modules.ModuleHook))]
    [HarmonyPatch("areModuleDependenciesEnabled")]
    public static class ModuleHookAreModuleDependenciesEnabledPatch
    {
        [HarmonyPrefix]
        private static void areModuleDependenciesEnabled(int moduleIndex)
        {
            var config = ModuleHook.modules[moduleIndex].config;
            ModuleHelper.FixRocketModDependencies(config);
        }
    }
}
