namespace OpenMod.Extensions.Games.Abstractions.Items
{
    /// <summary>
    /// Represents the state of an item.
    /// </summary>
    public interface IItemState
    {
        /// <value>
        /// The quality of the item.
        /// </value>
        double ItemQuality { get; }

        /// <value>
        /// The durability of the item.
        /// </value>
        double ItemDurability { get; }

        /// <value>
        /// The amount of the item.
        /// </value>
        double ItemAmount { get; }

        /// <summary>
        /// The state of the item. Can be null.
        /// </summary>
        byte[]? StateData { get; }
    }
}