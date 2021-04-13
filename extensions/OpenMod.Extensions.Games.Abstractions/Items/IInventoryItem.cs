using System.Threading.Tasks;

namespace OpenMod.Extensions.Games.Abstractions.Items
{
    /// <summary>
    /// Represents an item stored in the inventory.
    /// </summary>
    public interface IInventoryItem : IItemInstance
    {
        /// <summary>
        /// Drops the item to the ground.
        /// </summary>
        Task DropAsync();

        /// <summary>
        /// Destroys the item.
        /// </summary>
        Task<bool> DestroyAsync();
    }
}