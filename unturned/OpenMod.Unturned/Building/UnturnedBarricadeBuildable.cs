using Cysharp.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Entities;
using OpenMod.UnityEngine.Transforms;
using SDG.Unturned;
using System.Threading.Tasks;

namespace OpenMod.Unturned.Building
{
    public class UnturnedBarricadeBuildable : UnturnedBuildable, IDamageSource
    {
        public BarricadeData BarricadeData { get; }

        public BarricadeDrop BarricadeDrop { get; }

        public Interactable Interactable { get; }

        public UnturnedBarricadeBuildable(BarricadeData data, BarricadeDrop drop) : base(
            new UnturnedBuildableAsset(drop.asset),
            new UnityTransform(drop.model),
            new UnturnedBuildableState(data.barricade),
            new UnturnedBuildableOwnership(data),
            drop.instanceID.ToString())
        {
            BarricadeData = data;
            BarricadeDrop = drop;
            Interactable = drop.interactable;
        }

        public override Task DestroyAsync()
        {
            async UniTask DestroyTask()
            {
                await UniTask.SwitchToMainThread();

                if (BarricadeManager.tryGetInfo(BarricadeDrop.model, out var x, out var y, out var plant, out var index, out var region))
                {
                    BarricadeManager.destroyBarricade(region, x, y, plant, index);
                }
            }

            return DestroyTask().AsTask();
        }

        public string DamageSourceName
        {
            get
            {
                return ((UnturnedBuildableAsset)Asset).Asset.name;
            }
        }
    }
}