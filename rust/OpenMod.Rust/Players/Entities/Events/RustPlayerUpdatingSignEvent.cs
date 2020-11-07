using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Entities.Events
{
    public class RustPlayerUpdatingSignEvent : RustPlayerEvent, ICancellableEvent
    {
        public Signage Signage { get; }
        public bool IsCancelled { get; set; }
     
        public RustPlayerUpdatingSignEvent(
            RustPlayer player,
            Signage signage) : base(player)
        {
            Signage = signage;
        }
    }
}