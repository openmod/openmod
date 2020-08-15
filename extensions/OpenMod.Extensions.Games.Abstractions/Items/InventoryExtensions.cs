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
        public static IItem FindByType(this IInventory inventory, string itemType, IComparer<IItem> comparer = null)
        {
            var query = inventory.SelectMany(d => d.Items);
            return FindByType(query, itemType, comparer);
        }

        [CanBeNull]
        public static IItem FindByType(this IInventoryPage page, string itemType, IComparer<IItem> comparer = null)
        {
            return FindByType((IEnumerable<IItem>)page, itemType, comparer);
        }

        private static IItem FindByType(IEnumerable<IItem> query, string itemType, IComparer<IItem> comparer)
        {
            if (comparer != null)
            {
                query = query.OrderBy(x => x, comparer);
            }

            return query.FirstOrDefault(item => item.Asset.ItemType.Equals(itemType, StringComparison.OrdinalIgnoreCase));
        }

        [CanBeNull]
        public static IItem FindByAssetId(this IInventory inventory, string itemAssetId, IComparer<IItem> comparer = null)
        {
            var query = inventory.SelectMany(d => d.Items);
            return FindByAssetId(query, itemAssetId, comparer);
        }

        [CanBeNull]
        public static IItem FindByAssetId(this IInventoryPage page, string itemAssetId, IComparer<IItem> comparer = null)
        {
            return FindByAssetId((IEnumerable<IItem>)page, itemAssetId, comparer);

        }

        private static IItem FindByAssetId(IEnumerable<IItem> query, string itemType, IComparer<IItem> comparer)
        {
            if (comparer != null)
            {
                query = query.OrderBy(x => x, comparer);
            }

            return query.FirstOrDefault(item => item.Asset.ItemAssetId.Equals(itemType, StringComparison.OrdinalIgnoreCase));
        }
    }
}