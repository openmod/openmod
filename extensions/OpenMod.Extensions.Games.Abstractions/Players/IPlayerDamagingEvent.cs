using JetBrains.Annotations;
using OpenMod.API.Eventing;
using OpenMod.Extensions.Games.Abstractions.Entities;

namespace OpenMod.Extensions.Games.Abstractions.Players
{
    public interface IPlayerDamagingEvent : IPlayerEvent, ICancellableEvent
    {
        [CanBeNull]
        public IDamageSource DamageSource { get; }

        public double DamageAmount { get; set; }
    }
}