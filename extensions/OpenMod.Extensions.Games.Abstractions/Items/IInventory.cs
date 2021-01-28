using System.Collections.Generic;

namespace OpenMod.Extensions.Games.Abstractions.Items
{
    /// <summary>
    /// Represents an inventory.
    /// </summary>
    public interface IInventory : IReadOnlyCollection<IInventoryPage>
    {
        /// <summary>
        /// Gets the pages of the inventory.
        /// Page support depends on the game and owner object.
        /// Some games and objects only suppport a single page.
        /// </summary>
        /// <example>
        /// <para>In the game Unturned, the player inventory consists of multiple pages. These pages<br/> are:</para>
        /// * Hands<br/>
        /// * Clothing slots<br/>
        /// * Primary slot<br/>
        /// * Secondary slot<br/>
        /// * Clothing pages<br/>
        /// </example>
        IReadOnlyCollection<IInventoryPage> Pages { get; }

        /// <summary>
        /// Checks if the inventory is full.
        /// </summary>
        bool IsFull { get; }
    }
}