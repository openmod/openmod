using OpenMod.Extensions.Games.Abstractions.Entities;
using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Rust.Entities;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Life.Events
{
    public class RustPlayerDamagingEvent : RustPlayerEvent, IPlayerDamagingEvent
    {
        public HitInfo HitInfo { get; }

        public bool IsCancelled { get; set; }

        public IDamageSource? DamageSource { get; }

        public double DamageAmount { get; set; }

        public RustPlayerDamagingEvent(
            RustPlayer player,
            HitInfo hitInfo) : base(player)
        {
            HitInfo = hitInfo;
            DamageAmount = hitInfo.damageTypes.Total();
            DamageSource = hitInfo.InitiatorPlayer != null
                ? new RustPlayer(hitInfo.InitiatorPlayer)
                : hitInfo.Initiator != null
                    ? new RustEntity(hitInfo.Initiator)
                    : null;
        }
    }
}