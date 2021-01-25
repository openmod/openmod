using System.Numerics;

namespace OpenMod.Extensions.Games.Abstractions.Players
{
    /// <summary>
    /// The event that is triggered when a player death occurs.
    /// </summary>
    public interface IPlayerDeathEvent : IPlayerEvent
    {
        /// <summary>
        /// The position the player died at.
        /// </summary>
        Vector3 DeathPosition { get; }
    }
}