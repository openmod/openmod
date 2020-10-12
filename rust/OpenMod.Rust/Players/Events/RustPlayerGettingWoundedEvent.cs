using OpenMod.API.Eventing;

namespace OpenMod.Rust.Players.Events
{
    public class RustPlayerGettingWoundedEvent : RustPlayerEvent, ICancellableEvent
    {
        public RustPlayer Source { get; }

        public bool IsCancelled { get; set; }

        public RustPlayerGettingWoundedEvent(
            RustPlayer player,
            RustPlayer source) : base(player)
        {
            Source = source;
        }
    }
}