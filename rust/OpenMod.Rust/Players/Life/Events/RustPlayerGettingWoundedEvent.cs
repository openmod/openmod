using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Life.Events
{
    public class RustPlayerGettingWoundedEvent : RustPlayerEvent, ICancellableEvent
    {
        public RustPlayer? Source { get; }

        public bool IsCancelled { get; set; }

        public RustPlayerGettingWoundedEvent(
            RustPlayer player,
            RustPlayer? source) : base(player)
        {
            Source = source;
        }
    }
}