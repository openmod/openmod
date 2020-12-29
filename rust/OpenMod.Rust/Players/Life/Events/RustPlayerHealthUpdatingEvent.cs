using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Life.Events
{
    public class RustPlayerHealthUpdatingEvent : RustPlayerEvent, ICancellableEvent
    {
        public float OldValue { get; }
        public float NewValue { get; }

        public RustPlayerHealthUpdatingEvent(
            RustPlayer player,
            float oldValue, 
            float newValue) : base(player)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        public bool IsCancelled { get; set; }
    }
}