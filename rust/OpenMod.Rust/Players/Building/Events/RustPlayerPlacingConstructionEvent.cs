using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Building.Events
{
    public class RustPlayerPlacingConstructionEvent : RustPlayerEvent, ICancellableEvent
    {
        public Planner Planner { get; }
        public Construction Construction { get; }
        public bool IsCancelled { get; set; }

        public RustPlayerPlacingConstructionEvent(
            RustPlayer player,
            Planner planner, 
            Construction construction) : base(player)
        {
            Planner = planner;
            Construction = construction;
        }
    }
}