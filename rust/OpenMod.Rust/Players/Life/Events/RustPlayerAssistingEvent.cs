using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Life.Events
{
    public class RustPlayerAssistingEvent : RustPlayerEvent, ICancellableEvent
    {
        public RustPlayer Target { get; }
        public bool IsCancelled { get; set; }

        public RustPlayerAssistingEvent(
            RustPlayer player,
            RustPlayer target) : base(player)
        {
            Target = target;
        }
    }
}