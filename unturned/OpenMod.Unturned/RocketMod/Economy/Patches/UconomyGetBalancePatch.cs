namespace OpenMod.Unturned.RocketMod.Economy.Patches
{
    internal static class UconomyGetBalancePatch
    {
        public delegate void PreGetBalance(string id, ref decimal balance);
        public static event PreGetBalance? OnPreGetBalance;

        // ReSharper disable once InconsistentNaming
        public static bool GetBalancePrefix(string id, ref decimal __result)
        {
            OnPreGetBalance?.Invoke(id, ref __result);
            return false;
        }
    }
}