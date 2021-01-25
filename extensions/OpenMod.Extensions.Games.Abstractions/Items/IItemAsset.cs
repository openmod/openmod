using JetBrains.Annotations;

namespace OpenMod.Extensions.Games.Abstractions.Items
{
    /// <summary>
    /// Represents an item asset.
    /// </summary>
    public interface IItemAsset
    {
        /// <summary>
        /// The ID of the asset. Cannot be null.
        /// </summary>
        [NotNull]
        string ItemAssetId { get; }

        /// <summary>
        /// The human readable name of the asset. Cannot be null. 
        /// </summary>
        [NotNull]
        string ItemName { get; }

        /// <summary>
        /// The type of the item. Cannot be null.
        /// </summary>
        [NotNull]
        string ItemType { get; }
    }
}