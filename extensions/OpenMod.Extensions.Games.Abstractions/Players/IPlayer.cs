using JetBrains.Annotations;
using OpenMod.Extensions.Games.Abstractions.Entities;

namespace OpenMod.Extensions.Games.Abstractions.Players
{
    /// <summary>
    /// Represents a player entity.
    /// </summary>
    public interface IPlayer : IEntity
    {
        /// <summary>
        /// The stance of the player. Cannot be null.
        /// </summary>
        [NotNull]
        string Stance { get; }
    }
}