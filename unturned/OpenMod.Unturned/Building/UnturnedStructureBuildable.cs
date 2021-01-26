using Cysharp.Threading.Tasks;
using OpenMod.UnityEngine.Transforms;
using SDG.Unturned;
using System.Threading.Tasks;
using UnityEngine;

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

                if (StructureManager.tryGetInfo(StructureDrop.model, out var x, out var y, out var index, out var region))
                {
                    StructureManager.destroyStructure(region, x, y, index, Vector3.zero);
                }
            }

            return DestroyTask().AsTask();
        }
    }
}