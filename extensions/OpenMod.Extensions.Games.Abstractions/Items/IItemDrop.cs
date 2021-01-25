using System.Numerics;
using System.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Transforms;

namespace OpenMod.Extensions.Games.Abstractions.Items
{
    /// <summary>
    /// Represents an item dropped at ground.
    /// </summary>
    public interface IItemDrop : IItemInstance
    {
        /// <value>
        /// The position of the item.
        /// </value>
        public Vector3 Position { get; }

        /// <value>
        /// Destroys the item.
        /// </value>
        Task<bool> DestroyAsync();
    }
}