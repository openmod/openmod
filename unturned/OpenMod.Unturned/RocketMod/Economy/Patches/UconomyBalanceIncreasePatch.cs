namespace OpenMod.Unturned.RocketMod.Economy.Patches
{
    internal static class UconomyBalanceIncreasePatch
    {
        public delegate void IncreaseBalance(string id, decimal increaseBy, decimal newBalance);
        public static event IncreaseBalance OnIncreaseBalance;

        public static void IncreaseBalancePostfix(string id, decimal increaseBy, decimal __result)
        {
            OnIncreaseBalance?.Invoke(id, increaseBy, __result);
        }
    }
}