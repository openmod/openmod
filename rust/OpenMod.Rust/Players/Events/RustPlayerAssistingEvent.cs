using OpenMod.API.Eventing;

namespace OpenMod.Rust.Players.Events
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