using JetBrains.Annotations;
using OpenMod.Extensions.Games.Abstractions.Entities;

namespace OpenMod.Extensions.Games.Abstractions.Players
{
    public interface IPlayerDamagedEvent : IPlayerEvent
    {
        [CanBeNull]
        public IDamageSource DamageSource { get; }

        public double DamageAmount { get; }
    }
}
