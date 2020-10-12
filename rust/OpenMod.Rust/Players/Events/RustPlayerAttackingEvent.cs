using OpenMod.API.Eventing;

namespace OpenMod.Rust.Players.Events
{
    public class RustPlayerAttackingEvent : RustPlayerEvent, ICancellableEvent
    {
        public HitInfo HitInfo { get; }
        public bool IsCancelled { get; set; }
        
        public RustPlayerAttackingEvent(
            RustPlayer player, 
            HitInfo hitInfo) : base(player)
        {
            HitInfo = hitInfo;
        }
    }
}