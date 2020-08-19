using System;
using System.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Buildings;
using OpenMod.Extensions.Games.Abstractions.Transforms;
using OpenMod.UnityEngine.Transforms;
using SDG.Unturned;

namespace OpenMod.Unturned.Buildings
{
    public class UnturnedBarricadeBuildable : IBuildable
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
            throw new NotImplementedException();
        }
    }
}