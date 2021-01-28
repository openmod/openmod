using OpenMod.Extensions.Games.Abstractions.Transforms;

namespace OpenMod.Extensions.Games.Abstractions.Entities
{
    /// <summary>
    /// Represents a physical object in the game.
    /// </summary>
    public interface IGameObject
    {
        /// <summary>
        /// Gets the transform of the object.
        /// </summary>
        IWorldTransform Transform { get; }
    }
}