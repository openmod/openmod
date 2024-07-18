using HarmonyLib;
using OpenMod.Core.Patching;
using System.Reflection;
using System;

namespace OpenMod.Core
{
    [HarmonyPatch]
    public static class HarmonyUnpatchAllPatch
    {
        [HarmonyCleanup]
        public static Exception? Cleanup(Exception ex, MethodBase original)
        {
            HarmonyExceptionHandler.ReportCleanupException(typeof(HarmonyUnpatchAllPatch), ex, original);
            return null;
        }

        [HarmonyPatch(typeof(Harmony), nameof(Harmony.UnpatchAll))]
        [HarmonyPrefix]
        private static bool UnpatchAllPatch(Harmony __instance, string? harmonyID)
        {
            if (harmonyID != null)
            {
                return true;
            }

            __instance.UnpatchAll(__instance.Id);
            return false;
        }
    }
}
