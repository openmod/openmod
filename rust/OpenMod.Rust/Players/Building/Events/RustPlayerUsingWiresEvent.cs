using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Building.Events
{
    public class RustPlayerUsingWiresEvent : RustPlayerEvent, ICancellableEvent
    {
        public bool IsCancelled { get; set; }
     
        public RustPlayerUsingWiresEvent(RustPlayer player) : base(player)
        {
        }
    }
}