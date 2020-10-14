using HarmonyLib;
using SDG.Unturned;

namespace OpenMod.Unturned.Module.Shared.Patches
{
    [HarmonyPatch(typeof(Provider))]
    [HarmonyPatch("broadcastServerShutdown")]
    public static class UnturnedBroadcastShutdownPatch
    {
        [HarmonyPostfix]
        public static void AfterBroadcastShutdown()
        {
            SaveManager.save();
        }
    }
}
