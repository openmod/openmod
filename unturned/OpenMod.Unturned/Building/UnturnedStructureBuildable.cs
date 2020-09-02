using Cysharp.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Acl;
using OpenMod.Extensions.Games.Abstractions.Building;
using OpenMod.Extensions.Games.Abstractions.Transforms;
using OpenMod.UnityEngine.Transforms;
using SDG.Unturned;
using System.Threading.Tasks;
using Object = UnityEngine.Object;

namespace OpenMod.Unturned.Building
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
            async UniTask DestroyTask()
            {
                await UniTask.SwitchToMainThread();
                Object.Destroy(StructureDrop.model);
            }

            return DestroyTask().AsTask();
        }
    }
}