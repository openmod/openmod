using System.Collections.Generic;

namespace OpenMod.Extensions.Games.Abstractions.Items
{
    /// <summary>
    /// Represents a page of an inventory.
    /// <example>
    /// </example>
    /// </summary>
    public interface IInventoryPage : IReadOnlyCollection<IInventoryItem>
    {
        /// <summary>
        /// The inventory this page belongs to.
        /// </summary>
        IInventory Inventory { get; }

        /// <summary>
        /// Gets the name of this page.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the maximum amount of items this page can hold.
        /// </summary>
        int Capacity { get; }

        /// <summary>
        /// Checks if the player can only view the content.
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// Checks if the page is full.
        /// </summary>
        bool IsFull { get; }

        /// <summary>
        /// Gets the items this page holds.
        /// </summary>
        IReadOnlyCollection<IInventoryItem> Items { get; }
    }
}