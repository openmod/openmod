using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using OpenMod.Extensions.Games.Abstractions.Items;
using SDG.Unturned;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenMod.Unturned.Items
{
    [ServiceImplementation(Priority = Priority.Lowest)]
    public class UnturnedItemDirectory : IItemDirectory
    {
        public Task<IReadOnlyCollection<IItemAsset>> GetItemAssetsAsync()
        {
            var items = new List<ItemAsset>();
            Assets.find(items);

            var unturnedItems = items
                .ConvertAll(d => new UnturnedItemAsset(d));

            return Task.FromResult<IReadOnlyCollection<IItemAsset>>(unturnedItems);
        }

        public Task<IReadOnlyCollection<IItemDrop>> GetItemDropsAsync()
        {
            var drops = new List<IItemDrop>();

            for (byte x = 0; x < Regions.WORLD_SIZE; x++)
            {
                for (byte y = 0; y < Regions.WORLD_SIZE; y++)
                {
                    drops.AddRange(ItemManager.regions[x, y].items.Select(d => new UnturnedItemDrop(x, y, d)));
                }
            }

            return Task.FromResult<IReadOnlyCollection<IItemDrop>>(drops);
        }
    }
}