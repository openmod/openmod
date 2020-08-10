using System.Collections.Generic;

namespace OpenMod.Extensions.Games.Abstractions.Items
{
    public interface IInventoryPage : IReadOnlyCollection<IItem>
    {
        string Id { get; }

        bool IsReadOnly { get; }

        bool IsFull { get; }

        IReadOnlyCollection<IItem> Items { get; }
    }
}