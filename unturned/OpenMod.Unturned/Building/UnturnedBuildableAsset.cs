using OpenMod.Extensions.Games.Abstractions.Building;
using SDG.Unturned;

namespace OpenMod.Unturned.Building
{
    public class UnturnedBuildableAsset : IBuildableAsset
    {
        public string BuildableAssetId { get; }

        public string BuildableType { get; }

        public UnturnedBuildableAsset(Asset asset)
        {
            BuildableAssetId = asset.id.ToString();
            BuildableType = asset.GetType().Name;
        }
    }
}