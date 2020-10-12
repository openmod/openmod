using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.BuildingBlocks.Events
{
    public class RustPlayerDemolishingBuildingBlockEvent : RustPlayerEvent, ICancellableEvent
    {
        public BuildingBlock BuildingBlock { get; }

        public bool IsCancelled { get; set; }
     
        public RustPlayerDemolishingBuildingBlockEvent(
            RustPlayer player, 
            BuildingBlock buildingBlock) : base(player)
        {
            BuildingBlock = buildingBlock;
        }
    }
}