using OpenMod.Extensions.Games.Abstractions.Entities;

namespace OpenMod.Extensions.Games.Abstractions.Players
{
    /// <summary>
    /// The event that is triggered when damage has been dealt to a player.
    /// </summary>
    public interface IPlayerDamagedEvent : IPlayerEvent
    {
        /// <value>
        /// The damage source. Can be null.
        /// </value>
        public IDamageSource? DamageSource { get; }

        /// <summary>
        /// The amount of the damage.
        /// </summary>
        public double DamageAmount { get; }
    }
}
