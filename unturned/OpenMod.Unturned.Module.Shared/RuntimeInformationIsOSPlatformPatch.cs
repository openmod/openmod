using System.Runtime.InteropServices;
using HarmonyLib;
using UnityEngine;

namespace OpenMod.Unturned.Module.Shared
{
    // This patch is required because on Unity platforms this will always result in Windows being detected
    [HarmonyPatch(typeof(RuntimeInformation))]
    [HarmonyPatch(nameof(RuntimeInformation.IsOSPlatform))]
    public static class RuntimeInformationIsOSPlatformPatch
    {
        [HarmonyPrefix]
        public static bool IsOsPlatform(OSPlatform osPlatform, ref bool __result)
        {
            if (osPlatform == OSPlatform.OSX)
            {
                __result = Application.platform == RuntimePlatform.OSXPlayer;
                return false;
            }

            if (osPlatform == OSPlatform.Linux)
            {
                __result = Application.platform == RuntimePlatform.LinuxPlayer;
                return false;
            }
            
            if (osPlatform == OSPlatform.Windows)
            {
                __result = Application.platform == RuntimePlatform.WindowsPlayer;
                return false;
            }

            // Let original IsOsPlatform handle it
            return true;
        }
    }
}