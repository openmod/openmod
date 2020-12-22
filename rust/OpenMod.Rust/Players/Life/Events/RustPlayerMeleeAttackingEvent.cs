using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Life.Events
{
    public class RustPlayerMeleeAttackingEvent : RustPlayerEvent, ICancellableEvent
    {
        public HitInfo HitInfo { get; }
        public bool IsCancelled { get; set; }

        public RustPlayerMeleeAttackingEvent(
            RustPlayer player,
            HitInfo hitInfo) : base(player)
        {
            HitInfo = hitInfo;
        }
    }
}