using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using OpenMod.Extensions.Games.Abstractions.Building;
using SDG.Unturned;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace OpenMod.Unturned.Building
{
    [ServiceImplementation(Priority = Priority.Lowest)]
    public class UnturnedBuildableDirectory : IBuildableDirectory
    {
        public Task<IReadOnlyCollection<IBuildableAsset>> GetBuildableAssetsAsync()
        {
            var barricadeAssets = Assets.find(EAssetType.ITEM)
                .Where(k => k is ItemBarricadeAsset || k is ItemStructureAsset)
                .Select(d =>
                {
                    if (d is ItemBarricadeAsset barricadeAsset)
                        return new UnturnedBuildableAsset(barricadeAsset);
                    return new UnturnedBuildableAsset((ItemStructureAsset)d);
                })
                .ToList();

            return Task.FromResult<IReadOnlyCollection<IBuildableAsset>>(barricadeAssets);
        }

        public Task<IReadOnlyCollection<IBuildable>> GetBuildablesAsync()
        {
            async UniTask<IReadOnlyCollection<IBuildable>> GetBuildablesTask()
            {
                await UniTask.SwitchToMainThread();

                var barricadeRegions = BarricadeManager.regions.Cast<BarricadeRegion>()
                    .Concat(BarricadeManager.vehicleRegions);
                var structureRegions = StructureManager.regions.Cast<StructureRegion>();

                var barricadeDatas = barricadeRegions.SelectMany(brd => brd.barricades);
                var barricadeDrops = barricadeRegions.SelectMany(brd => brd.drops);
                var structureDatas = structureRegions.SelectMany(str => str.structures);
                var structureDrops = structureRegions.SelectMany(str => str.drops);

                return barricadeDatas
                    .Select(k =>
                    {
                        var drop = barricadeDrops.FirstOrDefault(l => l.instanceID == k.instanceID);
                        return drop == null ? null : new UnturnedBarricadeBuildable(k, drop);
                    })
                    .Cast<UnturnedBuildable>()
                    .Concat(structureDatas.Select(k =>
                    {
                        var drop = structureDrops.FirstOrDefault(l => l.instanceID == k.instanceID);
                        return drop == null ? null : new UnturnedStructureBuildable(k, drop);
                    }))
                    .Where(d => d != null)
                    .ToList();
            }
            
            return GetBuildablesTask().AsTask();
        }
    }
}