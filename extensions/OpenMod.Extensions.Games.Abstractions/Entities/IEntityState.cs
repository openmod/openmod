using JetBrains.Annotations;

namespace OpenMod.Extensions.Games.Abstractions.Entities
{
    /// <summary>
    /// Represents the state of an entity.
    /// </summary>
    public interface IEntityState
    {
        /// <summary>
        /// The state of the entity. Can be null.
        /// </summary>
        [CanBeNull]
        byte[] StateData { get; }
    }
}