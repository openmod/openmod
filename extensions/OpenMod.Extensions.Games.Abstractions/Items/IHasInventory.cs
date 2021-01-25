using JetBrains.Annotations;

namespace OpenMod.Extensions.Games.Abstractions.Items
{
    /// <summary>
    /// Represents an object that has inventory.
    /// </summary>
    public interface IHasInventory
    {
        /// <value>
        /// The inventory of the object.
        /// </value>
        [NotNull]
        IInventory Inventory { get; }
    }
}