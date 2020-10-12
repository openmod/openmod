using OpenMod.API.Eventing;

namespace OpenMod.Rust.Players.Events
{
    public class RustPlayerRevivingEvent : RustPlayerEvent, ICancellableEvent
    {
        public RustPlayer Reviver { get; }
        public bool IsCancelled { get; set; }

        public RustPlayerRevivingEvent(
            RustPlayer player,
            RustPlayer reviver) : base(player)
        {
            Reviver = reviver;
        }
    }
}