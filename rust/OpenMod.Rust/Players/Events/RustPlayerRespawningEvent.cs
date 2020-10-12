using OpenMod.API.Eventing;

namespace OpenMod.Rust.Players.Events
{
    public class RustPlayerRespawningEvent : RustPlayerEvent, ICancellableEvent
    {
        public bool IsCancelled { get; set; }
        public RustPlayerRespawningEvent(RustPlayer player) : base(player)
        {
        }
    }
}