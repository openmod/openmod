using System;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace OpenMod.Extensions.Games.Abstractions.Items
{
    public static class ItemDirectoryExtensions
    {
        [CanBeNull]
        public static async Task<IItemAsset> FindByIdAsync(this IItemDirectory directory, string itemId)
        {
            return (await directory.GetItemAssetsAsync())
                .FirstOrDefault(d => d.ItemAssetId.Equals(itemId, StringComparison.OrdinalIgnoreCase));
        }

        [CanBeNull]
        public static async Task<IItemAsset> FindByNameAsync(this IItemDirectory directory, string itemName, bool exact = true)
        {
            // todo: implement exact: false, find closest match

            return (await directory.GetItemAssetsAsync())
                .FirstOrDefault(d => d.ItemName.Equals(itemName, StringComparison.OrdinalIgnoreCase));
        }
    }
}