using System.Numerics;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace OpenMod.Extensions.Games.Abstractions.Items
{
    public static class ItemSpawnerExtensions
    {
        public static Task<IItemObject> GiveItemAsync(this IItemSpawner spawner, 
            IInventory inventory, 
            IItemAsset asset, 
            [CanBeNull] IItemState state = null)
        {
            return spawner.GiveItemAsync(inventory, asset.ItemAssetId, state);
        }

        public static Task<IItemDrop> SpawnItemAsync(this IItemSpawner spawner, 
            Vector3 position, 
            IItemAsset asset, 
            [CanBeNull] IItemState state = null)
        {
            return spawner.SpawnItemAsync(position, asset.ItemAssetId, state);
        }
    }
}