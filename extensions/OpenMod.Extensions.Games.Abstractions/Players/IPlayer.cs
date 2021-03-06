using System.Net;
using OpenMod.Extensions.Games.Abstractions.Entities;

namespace OpenMod.Extensions.Games.Abstractions.Players
{
    /// <summary>
    /// Represents a player entity.
    /// </summary>
    public interface IPlayer : IEntity
    {
        /// <summary>
        /// The stance of the player.
        /// </summary>
        string Stance { get; }

        /// <summary>
        /// The address of the player.
        /// </summary>
        IPAddress? Address { get; }
    }
}