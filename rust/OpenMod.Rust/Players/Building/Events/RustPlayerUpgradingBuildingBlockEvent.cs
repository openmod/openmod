using OpenMod.API.Eventing;
using OpenMod.Rust.Building;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Building.Events
{
    public class RustPlayerUpgradingBuildingBlockEvent : RustPlayerEvent, ICancellableEvent
    {
        public RustBuildingBlock BuildingBlock { get; }
        public ConstructionGrade Grade { get; }
        public bool IsCancelled { get; set; }

        public RustPlayerUpgradingBuildingBlockEvent(
            RustPlayer player,
            RustBuildingBlock buildingBlock,
            ConstructionGrade grade) : base(player)
        {
            BuildingBlock = buildingBlock;
            Grade = grade;
        }
    }
}