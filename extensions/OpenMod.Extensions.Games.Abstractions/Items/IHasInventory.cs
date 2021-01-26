namespace OpenMod.Extensions.Games.Abstractions.Items
{
    /// <summary>
    /// Represents an object that may have an inventory.
    /// </summary>
    public interface IHasInventory
    {
        /// <value>
        /// The inventory of the object.
        /// </value>
        IInventory? Inventory { get; }
    }
}