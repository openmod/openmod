using OpenMod.Extensions.Games.Abstractions.Building;
using SDG.Unturned;

namespace OpenMod.Unturned.Building
{
    public class UnturnedBuildableAsset : IBuildableAsset
    {
        public Asset Asset { get; }

        public string BuildableAssetId { get; }

        public string BuildableType { get; }

        public UnturnedBuildableAsset(ItemBarricadeAsset barricadeAsset) : this((Asset)barricadeAsset)
        {
            BuildableType = "barricade";
        }

        public UnturnedBuildableAsset(ItemStructureAsset structureAsset) : this((Asset)structureAsset)
        {
            BuildableType = "structure";
        }

        protected UnturnedBuildableAsset(Asset asset)
        {
            Asset = asset;
            BuildableAssetId = asset.id.ToString();
        }
    }
}