using OpenMod.API.Eventing;

namespace OpenMod.Rust.Players.Events
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