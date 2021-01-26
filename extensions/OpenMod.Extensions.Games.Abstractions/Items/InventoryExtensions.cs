using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenMod.Extensions.Games.Abstractions.Items
{
    public static class InventoryExtensions
    {
        /// <summary>
        /// Removes all items from the given inventory.
        /// </summary>
        /// <param name="inventory">The inventory to clear.</param>
        public static async Task ClearAsync(this IInventory inventory)
        {
            foreach (var item in inventory.SelectMany(d => d.Items))
            {
                await item.DestroyAsync();
            }
        }

        /// <summary>
        /// Searches for items by the item type.
        /// </summary>
        /// <param name="inventory">The inventory to search in.</param>
        /// <param name="itemType">The type of the item to search for.</param>
        /// <param name="comparer">The optional item comparer.</param>
        public static IEnumerable<IInventoryItem> FindByType(this IInventory inventory, string itemType, IComparer<IInventoryItem>? comparer = null)
        {
            var query = inventory.SelectMany(d => d.Items);
            return FindByType(query, itemType, comparer);
        }

        /// <summary>
        /// Searches for items by the item type.
        /// </summary>
        /// <param name="page">The inventory page to search in.</param>
        /// <param name="itemType">The type of the item to search for.</param>
        /// <param name="comparer">The optional item comparer.</param>
        public static IEnumerable<IInventoryItem> FindByType(this IInventoryPage page, string itemType, IComparer<IInventoryItem>? comparer = null)
        {
            return FindByType((IEnumerable<IInventoryItem>)page, itemType, comparer);
        }

        private static IEnumerable<IInventoryItem> FindByType(IEnumerable<IInventoryItem> query, string itemType, IComparer<IInventoryItem>? comparer)
        {
            if (comparer != null)
            {
                query = query.OrderBy(x => x, comparer);
            }

            return query.Where(item => item.Item.Asset.ItemType.Equals(itemType, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Searches for items by the item asset id.
        /// </summary>
        /// <param name="inventory">The inventory to search in.</param>
        /// <param name="itemAssetId">The ID of the item to search for.</param>
        /// <param name="comparer">The optional item comparer.</param>
        /// <returns></returns>
        public static IEnumerable<IInventoryItem> FindByAssetId(this IInventory inventory, string itemAssetId, IComparer<IInventoryItem>? comparer = null)
        {
            var query = inventory.SelectMany(d => d.Items);
            return FindByAssetId(query, itemAssetId, comparer);
        }

        /// <summary>
        /// Searches for items by the item asset id.
        /// </summary>
        /// <param name="page">The inventory page to search in.</param>
        /// <param name="itemAssetId">The ID of the item to search for.</param>
        /// <param name="comparer">The optional item comparer.</param>
        /// <returns></returns>
        public static IEnumerable<IInventoryItem> FindByAssetId(this IInventoryPage page, string itemAssetId, IComparer<IInventoryItem>? comparer = null)
        {
            return FindByAssetId((IEnumerable<IInventoryItem>)page, itemAssetId, comparer);
        }

        private static IEnumerable<IInventoryItem> FindByAssetId(IEnumerable<IInventoryItem> query, string itemType, IComparer<IInventoryItem>? comparer)
        {
            if (comparer != null)
            {
                query = query.OrderBy(x => x, comparer);
            }

            return query.Where(item => item.Item.Asset.ItemAssetId.Equals(itemType, StringComparison.OrdinalIgnoreCase));
        }
    }
}