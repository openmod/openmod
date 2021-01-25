using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.API.Ioc;

namespace OpenMod.Extensions.Games.Abstractions.Items
{
    /// <summary>
    /// The service for querying items and item assets.
    /// </summary>
    [Service]
    public interface IItemDirectory
    {
        /// <summary>
        /// Gets all item assets.
        /// </summary>
        Task<IReadOnlyCollection<IItemAsset>> GetItemAssetsAsync();

        /// <summary>
        /// Gets all items currently dropped on ground.
        /// </summary>
        Task<IReadOnlyCollection<IItemDrop>> GetItemDropsAsync();
    }
}