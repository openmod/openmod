using System.Collections.Generic;

namespace OpenMod.Extensions.Games.Abstractions.Items
{
    public interface IInventory : IReadOnlyCollection<IInventoryPage>
    {
        IReadOnlyCollection<IInventoryPage> Pages { get; }
    }
}