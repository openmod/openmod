namespace OpenMod.Extensions.Games.Abstractions.Items
{
    /// <summary>
    /// Represents the instance of an item.
    /// </summary>
    public interface IItemInstance
    {
        /// <value>
        /// The item.
        /// </value>
        IItem Item { get; }
    }
}