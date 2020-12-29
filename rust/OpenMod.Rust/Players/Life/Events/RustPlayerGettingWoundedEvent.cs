using JetBrains.Annotations;
using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Life.Events
{
    public class RustPlayerGettingWoundedEvent : RustPlayerEvent, ICancellableEvent
    {
        [CanBeNull]
        public RustPlayer Source { get; }

        public bool IsCancelled { get; set; }

        public RustPlayerGettingWoundedEvent(
            RustPlayer player,
            [CanBeNull] RustPlayer source) : base(player)
        {
            Source = source;
        }
    }
}