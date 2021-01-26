using System;
using System.Numerics;
using System.Threading.Tasks;

namespace OpenMod.Extensions.Games.Abstractions.Items
{
    public static class ItemSpawnerExtensions
    {
        /// See <see cref="IItemSpawner.GiveItemAsync"/>.
        public static Task<IItemInstance?> GiveItemAsync(this IItemSpawner spawner, 
            IInventory inventory, 
            IItemAsset asset, 
            IItemState? state = null)
        {
            if (spawner == null)
            {
                throw new ArgumentNullException(nameof(spawner));
            }

            return spawner.GiveItemAsync(inventory, asset.ItemAssetId, state);
        }

        /// See <see cref="IItemSpawner.GiveItemAsync"/>.
        public static Task<IItemDrop?> SpawnItemAsync(this IItemSpawner spawner, 
            Vector3 position, 
            IItemAsset asset, 
            IItemState? state = null)
        {
            if (spawner == null)
            {
                throw new ArgumentNullException(nameof(spawner));
            }

            return spawner.SpawnItemAsync(position, asset.ItemAssetId, state);
        }
    }
}