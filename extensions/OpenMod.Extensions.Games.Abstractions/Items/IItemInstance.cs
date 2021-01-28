namespace OpenMod.Extensions.Games.Abstractions.Items
{
    /// <summary>
    /// Represents the instance of an item.
    /// </summary>
    public interface IItemInstance
    {
        /// <summary>
        /// Gets the item.
        /// </summary>
        IItem Item { get; }
    }
}