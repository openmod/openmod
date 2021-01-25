using System.Collections.Generic;

namespace OpenMod.Extensions.Games.Abstractions.Items
{
    /// <summary>
    /// Represents an inventory.
    /// </summary>
    public interface IInventory : IReadOnlyCollection<IInventoryPage>
    {
        /// <value>
        /// The pages of the inventory.
        /// Page support depends on the game and owner object.
        /// Some games and objects only suppport a single page.
        /// </value>
        /// <example>
        /// <para>In the game Unturned, the player inventory consists of multiple pages. These pages<br/> are:</para>
        /// * Hands<br/>
        /// * Clothing slots<br/>
        /// * Primary slot<br/>
        /// * Secondary slot<br/>
        /// * Clothing pages<br/>
        /// </example>
        IReadOnlyCollection<IInventoryPage> Pages { get; }

        /// <value>
        /// <b>True</b> if the inventory is full; <b>false</b> otherwise.
        /// </value>
        bool IsFull { get; }
    }
}