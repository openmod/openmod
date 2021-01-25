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
        /// <value>
        /// The inventory this page belongs to.
        /// </value>
        IInventory Inventory { get; }

        /// <value>
        /// The name of this page.
        /// </value>
        string Name { get; }

        /// <value>
        /// The maximum amount of items this page can hold.
        /// </value>
        int Capacity { get; }

        /// <value>
        /// <b>True</b> if the player can only view items; otherwise, <b>false</b>.
        /// </value>
        bool IsReadOnly { get; }

        /// <value>
        /// <b>True</b> if the page is full; otherwise, <b>false</b>.
        /// </value>
        bool IsFull { get; }

        /// <value>
        /// The items this page holds.
        /// </value>
        IReadOnlyCollection<IInventoryItem> Items { get; }
    }
}