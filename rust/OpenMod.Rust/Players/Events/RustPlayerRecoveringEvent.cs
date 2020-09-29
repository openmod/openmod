using OpenMod.API.Eventing;

namespace OpenMod.Rust.Players.Events
{
    public class RustPlayerRecoveringEvent : RustPlayerEvent, ICancellableEvent
    {
        public bool IsCancelled { get; set; }
     
        public RustPlayerRecoveringEvent(RustPlayer player) : base(player)
        {
        }
    }
}