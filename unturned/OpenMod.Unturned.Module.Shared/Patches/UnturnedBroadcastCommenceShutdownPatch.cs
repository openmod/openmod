using System.Reflection;
using HarmonyLib;
using SDG.Unturned;

namespace OpenMod.Unturned.Module.Shared.Patches
{
    [HarmonyPatch(typeof(Provider))]
    [HarmonyPatch("onApplicationQuitting")]
    public static class UnturnedBroadcastCommenceShutdownPatch
    {
        private static readonly MethodInfo s_BroadcastCommenceShutdown = typeof(Provider).GetMethod("broadcastCommenceShutdown", BindingFlags.NonPublic | BindingFlags.Static);
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
        public static void BeforeDisconnect()
        {
            if (!Provider.isInitialized || s_ShutdownCommenced)
                return;

            s_BroadcastCommenceShutdown.Invoke(null, null);
        }
    }
}
