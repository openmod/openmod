using OpenMod.API.Eventing;

namespace OpenMod.Rust.Players.Events
{
    public class RustPlayerSleepEndingEvent : RustPlayerEvent, ICancellableEvent
    {
        public bool IsCancelled { get; set; }
     
        public RustPlayerSleepEndingEvent(RustPlayer player) : base(player)
        {
        }
    }
}