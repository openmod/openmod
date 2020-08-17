using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace OpenMod.Extensions.Games.Abstractions.Items
{
    public static class InventoryExtensions
    {
        public static async Task ClearAsync(this IInventory inventory)
        {
            foreach (var item in inventory.SelectMany(d => d.Items))
            {
                await item.DestroyAsync();
            }
        }

        [CanBeNull]
        public static IInventoryItem FindByType(this IInventory inventory, string itemType, IComparer<IInventoryItem> comparer = null)
        {
            var query = inventory.SelectMany(d => d.Items);
            return FindByType(query, itemType, comparer);
        }

        [CanBeNull]
        public static IInventoryItem FindByType(this IInventoryPage page, string itemType, IComparer<IInventoryItem> comparer = null)
        {
            return FindByType((IEnumerable<IInventoryItem>)page, itemType, comparer);
        }

        private static IInventoryItem FindByType(IEnumerable<IInventoryItem> query, string itemType, IComparer<IInventoryItem> comparer)
        {
            if (comparer != null)
            {
                query = query.OrderBy(x => x, comparer);
            }

            return query.FirstOrDefault(item => item.Item.Asset.ItemType.Equals(itemType, StringComparison.OrdinalIgnoreCase));
        }

        [CanBeNull]
        public static IInventoryItem FindByAssetId(this IInventory inventory, string itemAssetId, IComparer<IInventoryItem> comparer = null)
        {
            var query = inventory.SelectMany(d => d.Items);
            return FindByAssetId(query, itemAssetId, comparer);
        }

        [CanBeNull]
        public static IInventoryItem FindByAssetId(this IInventoryPage page, string itemAssetId, IComparer<IInventoryItem> comparer = null)
        {
            return FindByAssetId((IEnumerable<IInventoryItem>)page, itemAssetId, comparer);

        }

        private static IInventoryItem FindByAssetId(IEnumerable<IInventoryItem> query, string itemType, IComparer<IInventoryItem> comparer)
        {
            if (comparer != null)
            {
                query = query.OrderBy(x => x, comparer);
            }

            return query.FirstOrDefault(item => item.Item.Asset.ItemAssetId.Equals(itemType, StringComparison.OrdinalIgnoreCase));
        }
    }
}