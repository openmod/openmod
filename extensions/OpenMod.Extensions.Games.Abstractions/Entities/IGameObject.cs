using OpenMod.Extensions.Games.Abstractions.Transforms;

namespace OpenMod.Extensions.Games.Abstractions.Entities
{
    /// <summary>
    /// Represents a physical object in the game.
    /// </summary>
    public interface IGameObject
    {
        /// <value>
        /// The transform of the object.
        /// </value>
        IWorldTransform Transform { get; }
    }
}