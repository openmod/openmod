using System;
using System.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Buildings;
using OpenMod.Extensions.Games.Abstractions.Transforms;
using OpenMod.UnityEngine.Transforms;
using SDG.Unturned;

namespace OpenMod.Unturned.Buildings
{
    public class UnturnedStructureBuildable : IBuildable
    {
        public StructureData StructureData { get; }

        public StructureDrop StructureDrop { get; }

        public UnturnedStructureBuildable(StructureData data, StructureDrop drop)
        {
            StructureData = data;
            StructureDrop = drop;
            Asset = new UnturnedBuildableAsset(data.structure.asset);
            Transform = new UnityTransform(drop.model);
            State = new UnturnedBuildableState(data.structure);
            Ownership = new UnturnedBuildableOwnership(data);
            BuildableInstanceId = drop.instanceID.ToString();
        }

        public IBuildableAsset Asset { get; }

        public IWorldTransform Transform { get; }

        public IBuildableState State { get; }

        public IOwnership Ownership { get; }

        public string BuildableInstanceId { get; }

        public Task DestroyAsync()
        {
            throw new NotImplementedException();
        }
    }
}