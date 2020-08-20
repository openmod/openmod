using JetBrains.Annotations;
using OpenMod.API.Eventing;
using OpenMod.Extensions.Games.Abstractions.Entities;

namespace OpenMod.Extensions.Games.Abstractions.Players
{
    public class PlayerDamageEvent : PlayerEvent, ICancellableEvent
    {
        [CanBeNull]
        public IEntity Attacker { get; }

        public double DamageAmount { get; set; }

        public string DamageSource { get; set; }

        public bool IsCancelled { get; set; }

        public PlayerDamageEvent(IPlayer player, IEntity attacker, double damageAmount) : base(player)
        {
            Attacker = attacker;
            DamageAmount = damageAmount;
        }
    }
}