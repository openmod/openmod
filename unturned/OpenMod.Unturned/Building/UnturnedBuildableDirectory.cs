using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using OpenMod.Extensions.Games.Abstractions.Building;
using SDG.Unturned;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            // Note this is an intensive and slow piece of code when the barricade & structure count gets large.
            // Optimization would be best done by nelson or a full wrapper of the entire buildable system
            // that caches the buildables with a custom class.
            // If nelson does optimize, best would be to merge data & drops to a single class.
            var barricadeRegions = BarricadeManager.regions.Cast<BarricadeRegion>().ToList();
            barricadeRegions.AddRange(BarricadeManager.vehicleRegions.Cast<BarricadeRegion>());
            var structureRegions = StructureManager.regions.Cast<StructureRegion>().ToList();

            var barricadeDatas = barricadeRegions.SelectMany(brd => brd.barricades);
            var barricadeDrops = barricadeRegions.SelectMany(brd => brd.drops);
            var structureDatas = structureRegions.SelectMany(str => str.structures);
            var structureDrops = structureRegions.SelectMany(str => str.drops);

            var result = barricadeDatas.Select(k =>
            {
                var drop = barricadeDrops.FirstOrDefault(l => l.instanceID == k.instanceID);
                if (drop == null)
                    // Log warning/error of possible unsynced barricades
                    return null;

                return new UnturnedBarricadeBuildable(k, drop);
            }).ToList<IBuildable>();

            result.AddRange(structureDatas.Select(k =>
            {
                var drop = structureDrops.FirstOrDefault(l => l.instanceID == k.instanceID);
                if (drop == null)
                    // Log warning/error of possible unsynced structures
                    return null;

                return new UnturnedStructureBuildable(k, drop);
            }));

            return Task.FromResult<IReadOnlyCollection<IBuildable>>(result);
        }
    }
}