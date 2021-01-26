namespace OpenMod.Unturned.RocketMod.Patches
{
    internal static class RocketModPluginManagerPatches
    {
        public delegate void PostRocketPluginsLoaded();
        public static event PostRocketPluginsLoaded? OnPostRocketPluginsLoaded;

        public static void LoadPluginsPostfix()
        {
            OnPostRocketPluginsLoaded?.Invoke();
        }
    }
}