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
    }
}