using System.Collections.Generic;

namespace OpenMod.Extensions.Games.Abstractions.Items
{
    public interface IInventoryPage : IReadOnlyCollection<IInventoryItem>
    {
        IInventory Inventory { get; }

        string Name { get; }

        int Capacity { get; }

        bool IsReadOnly { get; }

        bool IsFull { get; }

        IReadOnlyCollection<IInventoryItem> Items { get; }
    }
}