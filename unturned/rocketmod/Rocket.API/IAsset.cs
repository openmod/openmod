namespace Rocket.API
{
    public delegate void AssetLoaded<T>(IAsset<T> asset) where T : class;
    public delegate void AssetUnloaded<T>(IAsset<T> asset) where T : class;

    public interface IAsset<T> where T : class
    {
        T Instance { get; set; }
        T Save();
        void Load(AssetLoaded<T> callback = null);
        void Unload(AssetUnloaded<T> callback = null);
    }
}
