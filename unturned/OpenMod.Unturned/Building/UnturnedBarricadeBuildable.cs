using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Entities;
using OpenMod.UnityEngine.Transforms;
using SDG.Unturned;

namespace OpenMod.Unturned.Building
{
    public class UnturnedBarricadeBuildable : UnturnedBuildable, IDamageSource
    {
        public BarricadeData BarricadeData
        {
            get
            {
                return BarricadeDrop.GetServersideData();
            }
        }

        public BarricadeDrop BarricadeDrop { get; }

        public Interactable Interactable { get; }

        [Obsolete("Use another constructor with only BarricadeDrop")]
        public UnturnedBarricadeBuildable(BarricadeData data, BarricadeDrop drop) : this(drop)
        {
            if (drop.GetServersideData() != data)
            {
                throw new Exception($"{nameof(data)} is incorrect barricade data for {nameof(drop)}");
            }
        }

        public UnturnedBarricadeBuildable(BarricadeDrop drop) : base(
            new UnturnedBuildableAsset(drop.asset),
            new UnityTransform(drop.model),
            new UnturnedBuildableState(drop.GetServersideData().barricade),
            new UnturnedBuildableOwnership(drop.GetServersideData()),
            drop.instanceID.ToString())
        {
            BarricadeDrop = drop;
            Interactable = drop.interactable;
        }

        public override Task DestroyAsync()
        {
            async UniTask DestroyTask()
            {
                await UniTask.SwitchToMainThread();

                if (BarricadeDrop.GetNetId().IsNull())
                {
                    return;
                }

                if (BarricadeManager.tryGetRegion(BarricadeDrop.model, out var x, out var y, out var plant, out _))
                {
                    BarricadeManager.destroyBarricade(BarricadeDrop, x, y, plant);
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