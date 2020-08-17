namespace OpenMod.Extensions.Games.Abstractions.Items
{
    public interface IHasInventory
    {
        IInventory Inventory { get; }
    }
}