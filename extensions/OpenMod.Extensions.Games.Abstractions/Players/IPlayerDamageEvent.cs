using JetBrains.Annotations;
using OpenMod.API.Eventing;
using OpenMod.Extensions.Games.Abstractions.Entities;

namespace OpenMod.Extensions.Games.Abstractions.Players
{
    public interface IPlayerDamageEvent : IPlayerEvent, ICancellableEvent
    {
        [CanBeNull]
        public IEntity Attacker { get; }

        public double DamageAmount { get; set; }

        public string DamageSource { get; set; }
    }
}