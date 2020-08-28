using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Acl;
using OpenMod.Extensions.Games.Abstractions.Building;
using OpenMod.Extensions.Games.Abstractions.Entities;
using OpenMod.Extensions.Games.Abstractions.Transforms;
using OpenMod.UnityEngine.Transforms;
using SDG.Unturned;
using Object = UnityEngine.Object;

namespace OpenMod.Unturned.Building
{
    public class UnturnedBarricadeBuildable : IBuildable, IDamageSource
    {
        public BarricadeData BarricadeData { get; }

        public BarricadeDrop BarricadeDrop { get; }

        public UnturnedBarricadeBuildable(BarricadeData data, BarricadeDrop drop)
        {
            BarricadeData = data;
            BarricadeDrop = drop;
            Asset = new UnturnedBuildableAsset(drop.asset);
            Transform = new UnityTransform(drop.model);
            State = new UnturnedBuildableState(data.barricade);
            Ownership = new UnturnedBuildableOwnership(data);
            BuildableInstanceId = drop.instanceID.ToString();
            Interactable = drop.interactable;
        }

        public IBuildableAsset Asset { get; }

        public IWorldTransform Transform { get; }

        public IBuildableState State { get; }

        public IOwnership Ownership { get; }

        public string BuildableInstanceId { get; }

        public Interactable Interactable { get; }

        public Task DestroyAsync()
        {
            async UniTask DestroyTask()
            {
                await UniTask.SwitchToMainThread();
                Object.Destroy(Interactable);
            }

            return DestroyTask().AsTask();
        }
    }
}