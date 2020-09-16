using HarmonyLib;
using SDG.Framework.Modules;

namespace Rocket.Unturned.Patches
{
    [HarmonyPatch(typeof(ModuleHook))]
    [HarmonyPatch("areModuleDependenciesEnabled")]
    public static class ModuleHookAreModuleDependenciesEnabledPatch
    {
        [HarmonyPrefix]
        private static void areModuleDependenciesEnabled(int moduleIndex)
        {
            var config = ModuleHook.modules[moduleIndex].config;
            ModuleHelper.RedirectRocketModDependencies(config);
        }
    }
}
