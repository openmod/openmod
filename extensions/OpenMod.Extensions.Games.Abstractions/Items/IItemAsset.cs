namespace OpenMod.Extensions.Games.Abstractions.Items
{
    /// <summary>
    /// Represents an item asset.
    /// </summary>
    public interface IItemAsset
    {
        /// <summary>
        /// The ID of the asset.
        /// </summary>
        string ItemAssetId { get; }

        /// <summary>
        /// The human readable name of the asset.
        /// </summary>
        string ItemName { get; }

        /// <summary>
        /// The type of the item.
        /// </summary>
        string ItemType { get; }

        /// <summary>
        /// The maximum quality of the item. <c>null</c> if item doesn't support qualities.
        /// </summary>
        double? MaxQuality { get; }

        /// <summary>
        /// The maximum quality of the item. <c>null</c> if item doesn't support amounts.
        /// </summary>
        double? MaxAmount { get; }

        /// <summary>
        /// The maximum durability of the item. <c>null</c> if item doesn't support durabilities.
        /// </summary>
        double? MaxDurability { get; }
    }
}