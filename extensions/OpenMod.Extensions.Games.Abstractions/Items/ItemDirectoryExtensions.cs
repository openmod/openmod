using OpenMod.Core.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OpenMod.Extensions.Games.Abstractions.Items
{
    public static class ItemDirectoryExtensions
    {
        /// <summary>
        /// Searches for items by the item asset id.
        /// </summary>
        /// <param name="directory">The item directory service.</param>
        /// <param name="itemAssetId">The item asset id to search for.</param>
        /// <returns><b>The <see cref="IItemAsset"/></b> if found; otherwise, <b>null</b>.</returns>
        public static async Task<IItemAsset?> FindByIdAsync(this IItemDirectory directory, string itemAssetId)
        {
            return (await directory.GetItemAssetsAsync())
                .FirstOrDefault(d => d.ItemAssetId.Equals(itemAssetId, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Searches for items by the item asset name.
        /// </summary>
        /// <param name="directory">The item directory service.</param>
        /// <param name="itemName">The name of the item asset.</param>
        /// <param name="exact">If true, only exact name matches will be used.</param>
        /// <returns><b>The <see cref="IItemAsset"/></b> if found; otherwise, <b>null</b>.</returns>
        public static async Task<IItemAsset?> FindByNameAsync(this IItemDirectory directory, string itemName, bool exact = true)
        {
            if (exact)
                return (await directory.GetItemAssetsAsync()).FirstOrDefault(d =>
                    d.ItemName.Equals(itemName, StringComparison.OrdinalIgnoreCase));

            var matches = (await directory.GetItemAssetsAsync())
                .Where(x => x.ItemName.IndexOf(itemName, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToArray();

            var minDist = int.MaxValue;
            IItemAsset? match = null;

            foreach (var asset in matches)
            {
                var distance = StringHelper.LevenshteinDistance(itemName, asset.ItemName);

                // There's no lower distance
                if (distance == 0)
                    return asset;

                if (match == null || distance < minDist)
                {
                    match = asset;
                    minDist = distance;
                }
            }

            return match;
        }
    }
}