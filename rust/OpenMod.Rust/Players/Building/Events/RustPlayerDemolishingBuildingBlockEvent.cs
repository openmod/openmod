using OpenMod.API.Eventing;
using OpenMod.Rust.Building;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Building.Events
{
    public class RustPlayerDemolishingBuildingBlockEvent : RustPlayerEvent, ICancellableEvent
    {
        public RustBuildingBlock BuildingBlock { get; }

        public bool IsCancelled { get; set; }

        public RustPlayerDemolishingBuildingBlockEvent(
            RustPlayer player,
            RustBuildingBlock buildingBlock) : base(player)
        {
            BuildingBlock = buildingBlock;
        }
    }
}