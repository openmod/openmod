using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using SDG.Unturned;

namespace OpenMod.Unturned.Module.Shared.Patches
{
    [HarmonyPatch(typeof(Provider))]
    [HarmonyPatch("onApplicationQuitting")]
    public static class UnturnedBroadcastCommenceShutdownPatch
    {
        private static readonly MethodInfo s_BroadcastCommenceShutdown = AccessTools.Method(typeof(Provider), "broadcastCommenceShutdown");
        private static bool s_ShutdownCommenced;

        static UnturnedBroadcastCommenceShutdownPatch()
        {
            Provider.onCommenceShutdown += OnCommenceShutdown;
        }

        private static void OnCommenceShutdown()
        {
            s_ShutdownCommenced = true;
        }

        [HarmonyPrefix]
        [UsedImplicitly]
        public static void BeforeDisconnect()
        {
            if (!Provider.isInitialized || s_ShutdownCommenced)
            {
                return;
            }

            s_BroadcastCommenceShutdown.Invoke(null, null);
        }
    }
}
