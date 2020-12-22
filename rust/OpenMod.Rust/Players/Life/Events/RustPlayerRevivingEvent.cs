using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Life.Events
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