using System;
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
            Asset = asset ?? throw new ArgumentNullException(nameof(asset));
            BuildableAssetId = asset.id.ToString();

            BuildableType = asset switch
            {
                ItemStructureAsset => "structure",
                ItemBarricadeAsset => "barricade",
                _ => throw new ArgumentException(
                    $"The given asset is not a structure or barricade asset. Id: {asset.id}, type: {asset.GetType().Name}",
                    nameof(asset))
            };
        }
    }
}