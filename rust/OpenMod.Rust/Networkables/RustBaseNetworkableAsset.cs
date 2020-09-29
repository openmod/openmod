using OpenMod.Extensions.Games.Abstractions.Building;

namespace OpenMod.Rust.Networkables
{
    public class RustBaseNetworkableAsset : IBuildableAsset
    {
        public const string BuildingBlock = "building_block";

        public RustBaseNetworkableAsset(BaseNetworkable networkable, string type)
        {
            BuildableAssetId = networkable.PrefabName;
            BuildableType = type;
        }

        public string BuildableAssetId { get; }

        public string BuildableType { get; }
    }
}