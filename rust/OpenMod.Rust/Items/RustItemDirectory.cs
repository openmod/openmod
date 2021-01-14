using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using OpenMod.Extensions.Games.Abstractions.Items;

namespace OpenMod.Rust.Items
{
    [ServiceImplementation(Priority = Priority.Lowest)]
    public class RustItemDirectory : IItemDirectory
    {
        public Task<IReadOnlyCollection<IItemAsset>> GetItemAssetsAsync()
        {
            var items = ItemManager.itemList
                .Select(d => new RustItemAsset(d))
                .ToList();

            return Task.FromResult<IReadOnlyCollection<IItemAsset>>(items);
        }

        public Task<IReadOnlyCollection<IItemDrop>> GetItemDropsAsync()
        {
            var items = BaseNetworkable.serverEntities
                .OfType<DroppedItem>()
                .Select(x => new RustItemDrop(x))
                .ToList();

            return Task.FromResult<IReadOnlyCollection<IItemDrop>>(items);
        }
    }
}
