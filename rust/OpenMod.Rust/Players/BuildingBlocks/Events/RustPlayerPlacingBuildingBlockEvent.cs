using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.BuildingBlocks.Events
{
    public class RustPlayerPlacingBuildingBlockEvent : RustPlayerEvent, ICancellableEvent
    {
        public Planner Planner { get; }
        public Construction Construction { get; }
        public bool IsCancelled { get; set; }

        public RustPlayerPlacingBuildingBlockEvent(
            RustPlayer player,
            Planner planner, 
            Construction construction) : base(player)
        {
            Planner = planner;
            Construction = construction;
        }
    }
}