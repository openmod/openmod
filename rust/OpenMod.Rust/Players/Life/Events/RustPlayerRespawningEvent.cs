using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Life.Events
{
    public class RustPlayerRespawningEvent : RustPlayerEvent, ICancellableEvent
    {
        public bool IsCancelled { get; set; }
        public RustPlayerRespawningEvent(RustPlayer player) : base(player)
        {
        }
    }
}