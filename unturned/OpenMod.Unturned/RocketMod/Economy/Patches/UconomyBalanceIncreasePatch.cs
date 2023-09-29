namespace OpenMod.Unturned.RocketMod.Economy.Patches
{
    internal static class UconomyBalanceIncreasePatch
    {
        public delegate void PostIncreaseBalance(string id, decimal increaseBy, decimal newBalance);
        public static event PostIncreaseBalance? OnPostIncreaseBalance;

        // ReSharper disable once InconsistentNaming
        public static void IncreaseBalancePostfix(string id, decimal increaseBy, decimal __result)
        {
            OnPostIncreaseBalance?.Invoke(id, increaseBy, __result);
        }

        public delegate void PreIncreaseBalance(string id, decimal increaseBy, ref decimal newBalance);
        public static event PreIncreaseBalance? OnPreIncreaseBalance;

        // ReSharper disable once InconsistentNaming
        public static bool IncreaseBalancePrefix(string id, decimal increaseBy, ref decimal __result)
        {
            OnPreIncreaseBalance?.Invoke(id, increaseBy, ref __result);
            return false;
        }
    }
}