using OpenMod.Extensions.Games.Abstractions.Items;

namespace OpenMod.Extensions.Games.Abstractions.Entities
{
    public interface IHasInventory
    {
        IInventory Inventory { get; }
    }
}