using System.Numerics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using OpenMod.API.Ioc;

namespace OpenMod.Extensions.Games.Abstractions.Items
{
    /// <summary>
    /// The service for spawning items.
    /// </summary>
    [Service]
    public interface IItemSpawner
    {
        /// <summary>
        /// Adds an item to the given inventory. The item will be dropped if the inventory is full.
        /// </summary>
        /// <param name="inventory">The inventory to add the item to.</param>
        /// <param name="itemAssetId">The ID of the item asset.</param>
        /// <param name="state">The optional state of the item.</param>
        /// <returns><b><see cref="IInventoryItem"/></b> if the inventory was not full; otherwise, <see cref="IItemDrop"/>. If spawning was not successful, <b>null</b>.</returns>
        [CanBeNull]
        Task<IItemInstance> GiveItemAsync(IInventory inventory, string itemAssetId, [CanBeNull] IItemState state = null);

        /// <summary>
        /// Spawns an item.
        /// </summary>
        /// <param name="position">The position to spawn the item at.</param>
        /// <param name="itemAssetId">The ID of the item asset.</param>
        /// <param name="state">The optional state of the item.</param>
        /// <returns><b>The dropped item</b> if successful; otherwise, <b>null</b>.</returns>
        [CanBeNull]
        Task<IItemDrop> SpawnItemAsync(Vector3 position, string itemAssetId, [CanBeNull] IItemState state = null);
    }
}