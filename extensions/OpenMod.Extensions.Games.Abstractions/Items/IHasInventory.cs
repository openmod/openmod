namespace OpenMod.Extensions.Games.Abstractions.Items
{
    /// <summary>
    /// Represents an object that may have an inventory.
    /// </summary>
    public interface IHasInventory
    {
        /// <summary>
        /// Gets the inventory of the object.
        /// </summary>
        IInventory? Inventory { get; }
    }
}