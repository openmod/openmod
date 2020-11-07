using System.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Acl;
using OpenMod.Extensions.Games.Abstractions.Building;
using OpenMod.Extensions.Games.Abstractions.Transforms;
using OpenMod.Rust.Networkables;
using OpenMod.UnityEngine.Transforms;

namespace OpenMod.Rust.Building
{
    public class RustBuildingBlock : IBuildable
    {
        public BuildingBlock BuildingBlock { get; }

        public RustBuildingBlock(BuildingBlock buildingBlock)
        {
            BuildingBlock = buildingBlock;
            Asset = new RustBaseNetworkableAsset(buildingBlock, RustBaseNetworkableAsset.BuildingBlock);
            Transform = new UnityTransform(buildingBlock.transform);
            State = new RustBuildingBlockState(buildingBlock);
            BuildableInstanceId = buildingBlock.GetInstanceID().ToString();

            // Rust todo: set Ownership
        }

        public IOwnership Ownership { get; }

        public IBuildableAsset Asset { get; }

        public IWorldTransform Transform { get; }

        public IBuildableState State { get; }

        public string BuildableInstanceId { get; }

        public Task DestroyAsync()
        {
            // Rust todo: implemented replicated
            throw new System.NotImplementedException();
        }
    }
}