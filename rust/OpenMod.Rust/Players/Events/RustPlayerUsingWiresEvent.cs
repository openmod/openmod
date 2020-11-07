using OpenMod.API.Eventing;

namespace OpenMod.Rust.Players.Events
{
    public class RustPlayerUsingWiresEvent : RustPlayerEvent, ICancellableEvent
    {
        public bool IsCancelled { get; set; }
     
        public RustPlayerUsingWiresEvent(RustPlayer player) : base(player)
        {
        }
    }
}