using OpenMod.API.Eventing;
using OpenMod.Extensions.Games.Abstractions.Entities;

namespace OpenMod.Extensions.Games.Abstractions.Players
{
    /// <summary>
    /// The event that is triggered when damage is to be dealt to a player.
    /// </summary>
    public interface IPlayerDamagingEvent : IPlayerEvent, ICancellableEvent
    {
        /// <summary>
        /// The damage source.
        /// </summary>
        public IDamageSource? DamageSource { get; }

        /// <summary>
        /// The amount of the damage.
        /// </summary>
        public double DamageAmount { get; set; }
    }
}