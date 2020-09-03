using Cysharp.Threading.Tasks;
using OpenMod.UnityEngine.Transforms;
using SDG.Unturned;
using System.Threading.Tasks;
using Object = UnityEngine.Object;

namespace OpenMod.Unturned.Building
{
    public class UnturnedStructureBuildable : UnturnedBuildable
    {
        public StructureData StructureData { get; }

        public StructureDrop StructureDrop { get; }

        public UnturnedStructureBuildable(StructureData data, StructureDrop drop) : base(
            new UnturnedBuildableAsset(data.structure.asset),
            new UnityTransform(drop.model),
            new UnturnedBuildableState(data.structure),
            new UnturnedBuildableOwnership(data),
            drop.instanceID.ToString())
        {
            StructureData = data;
            StructureDrop = drop;
        }

        public override Task DestroyAsync()
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