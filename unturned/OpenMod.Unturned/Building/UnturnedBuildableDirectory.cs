using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using OpenMod.Extensions.Games.Abstractions.Building;
using SDG.Unturned;

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
                    .Concat(BarricadeManager.vehicleRegions)
                    .ToList();

                var structureRegions = StructureManager.regions.Cast<StructureRegion>()
                    .ToList();

                var barricadeDrops = barricadeRegions.SelectMany(brd => brd.drops).ToList();
                var structureDrops = structureRegions.SelectMany(str => str.drops).ToList();

                return barricadeDrops
                    .Select(d => new UnturnedBarricadeBuildable(d))
                    .Cast<UnturnedBuildable>()
                    .Concat(structureDrops.Select(d => new UnturnedStructureBuildable(d)))
                    .Select(d => d!)
                    .ToList();
            }

            return GetBuildablesTask().AsTask();
        }
    }
}
