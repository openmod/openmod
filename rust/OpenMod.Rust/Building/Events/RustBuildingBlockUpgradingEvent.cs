using OpenMod.API.Eventing;
using OpenMod.Rust.Players;

namespace OpenMod.Rust.Building.Events
{
    public class RustBuildingBlockUpgradingEvent : RustBuildingBlockEvent, ICancellableEvent
    {
        public RustBuildingBlockUpgradingEvent(
            RustBuildingBlock buildingBlock,
            RustPlayer player,
            BuildingGrade.Enum grade) : base(buildingBlock)
        {
            Player = player;
            Grade = grade;
        }

        public RustPlayer Player { get; }

        public BuildingGrade.Enum Grade { get; }

        public bool IsCancelled { get; set; }
    }
}