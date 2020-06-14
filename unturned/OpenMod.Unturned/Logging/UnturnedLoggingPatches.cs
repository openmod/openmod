using HarmonyLib;
using Microsoft.Extensions.Logging;
using SDG.Unturned;

namespace OpenMod.Unturned.Logging
{
    internal static class UnturnedLoggingPatches
    {
        public static ILogger Logger { get; set; }
    }

    [HarmonyPatch(typeof(ConsoleInputOutput))]
    [HarmonyPatch(nameof(ConsoleInputOutput.outputInformation))]
    public static class WindowsConsole_conditionalAlloc
    {
        [HarmonyPrefix]
        public static bool outputInformation(string message)
        {
            UnturnedLoggingPatches.Logger.LogInformation(message);
            return false;
        }
    }

    [HarmonyPatch(typeof(ConsoleInputOutput))]
    [HarmonyPatch(nameof(ConsoleInputOutput.outputWarning))]
    public static class ConsoleInputOutput_outputWarning_Patch
    {
        [HarmonyPrefix]
        public static bool outputWarning(string message)
        {
            UnturnedLoggingPatches.Logger.LogWarning(message);
            return false;
        }
    }
}