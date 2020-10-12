using OpenMod.API.Eventing;

namespace OpenMod.Rust.Players.Events
{
    public class RustPlayerSleepStartingEvent : RustPlayerEvent, ICancellableEvent
    {
        public bool IsCancelled { get; set; }
     
        public RustPlayerSleepStartingEvent(RustPlayer player) : base(player)
        {
        }
    }
}