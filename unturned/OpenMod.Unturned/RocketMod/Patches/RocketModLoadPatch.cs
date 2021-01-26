namespace OpenMod.Unturned.RocketMod.Patches
{
    internal static class RocketModLoadPatch
    {
        public delegate void RocketModIntialized();
        public static event RocketModIntialized? OnRocketModIntialized;

        public static void PostInitialize()
        {
            OnRocketModIntialized?.Invoke();
        }
    }
}