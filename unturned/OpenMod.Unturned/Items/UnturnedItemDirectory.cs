using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using OpenMod.Extensions.Games.Abstractions.Items;
using SDG.Unturned;

namespace OpenMod.Unturned.Items
{
    [ServiceImplementation(Priority = Priority.Lowest)]
    public class UnturnedItemDirectory : IItemDirectory
    {
        public Task<IReadOnlyCollection<IItemAsset>> GetItemAssetsAsync()
        {
            var items = Assets.find(EAssetType.ITEM)
                .Cast<ItemAsset>()
                .Select(d => new UnturnedItemAsset(d))
                .ToList();

            return Task.FromResult<IReadOnlyCollection<IItemAsset>>(items);
        }

        public Task<IReadOnlyCollection<IItemDrop>> GetItemDropsAsync()
        {
            var drops = new List<IItemDrop>();

            foreach (var region in ItemManager.regions)
            {
                drops.AddRange(region.drops.Select(d => new UnturnedItemDrop(region, d)));
            }

            return Task.FromResult<IReadOnlyCollection<IItemDrop>>(drops);
        }
    }
}