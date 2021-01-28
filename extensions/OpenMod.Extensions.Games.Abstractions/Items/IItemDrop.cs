using System.Numerics;
using System.Threading.Tasks;

namespace OpenMod.Extensions.Games.Abstractions.Items
{
    /// <summary>
    /// Represents an item dropped at ground.
    /// </summary>
    public interface IItemDrop : IItemInstance
    {
        /// <summary>
        /// Gets the position of the item.
        /// </summary>
        public Vector3 Position { get; }

        /// <summary>
        /// Destroys the item.
        /// </summary>
        Task<bool> DestroyAsync();
    }
}