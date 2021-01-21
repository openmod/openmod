using System;

namespace OpenMod.Unturned.RocketMod.Economy.Patches
{
    internal static class UconomyExecuteQueryPatch
    {
        public static void ExecuteQueryPrefix()
        {
            throw new NotSupportedException("ExecuteQuery not supported with OpenMod_Uconomy.");
        }
    }
}