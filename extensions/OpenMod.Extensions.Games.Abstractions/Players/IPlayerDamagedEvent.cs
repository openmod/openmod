using OpenMod.Extensions.Games.Abstractions.Entities;

namespace OpenMod.Extensions.Games.Abstractions.Players
{
    /// <summary>
    /// The event that is triggered when damage has been dealt to a player.
    /// </summary>
    public interface IPlayerDamagedEvent : IPlayerEvent
    {
        /// <summary>
        /// GEts the damage source.
        /// </summary>
        public IDamageSource? DamageSource { get; }

        /// <summary>
        /// Gets the amount of the damage dealt.
        /// </summary>
        public double DamageAmount { get; }
    }
}
