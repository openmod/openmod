namespace OpenMod.Extensions.Games.Abstractions.Items
{
    /// <summary>
    /// Represents the state of an item.
    /// </summary>
    public interface IItemState
    {
        /// <summary>
        /// Gets the quality of the item.
        /// </summary>
        double ItemQuality { get; }

        /// <summary>
        /// Gets the durability of the item.
        /// </summary>
        double ItemDurability { get; }

        /// <summary>
        /// Gets the amount of the item.
        /// </summary>
        double ItemAmount { get; }

        /// <summary>
        /// Gets the state of the item.
        /// </summary>
        byte[]? StateData { get; }
    }
}