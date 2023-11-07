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
            var assets = new List<ItemPlaceableAsset>();
            Assets.find(assets);

            var placeablesAssets = assets
                .ConvertAll(d => new UnturnedBuildableAsset(d));

            return Task.FromResult<IReadOnlyCollection<IBuildableAsset>>(placeablesAssets);
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

                var buildables = barricadeDrops
                    .ConvertAll<UnturnedBuildable>(d => new UnturnedBarricadeBuildable(d));

                buildables.AddRange(
                        structureDrops.ConvertAll(d => new UnturnedStructureBuildable(d)));

                return buildables;
            }

            return GetBuildablesTask().AsTask();
        }
    }
}
