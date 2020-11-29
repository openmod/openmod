using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Life.Events
{
    public class RustPlayerSleepEndingEvent : RustPlayerEvent, ICancellableEvent
    {
        public bool IsCancelled { get; set; }
     
        public RustPlayerSleepEndingEvent(RustPlayer player) : base(player)
        {
        }
    }
}