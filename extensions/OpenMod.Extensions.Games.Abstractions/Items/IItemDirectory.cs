using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.API.Ioc;

namespace OpenMod.Extensions.Games.Abstractions.Items
{
    [Service]
    public interface IItemDirectory
    {
        Task<IReadOnlyCollection<IItemAsset>> GetItemAssetsAsync();

        Task<IReadOnlyCollection<IItemDrop>> GetItemDropsAsync();
    }
}