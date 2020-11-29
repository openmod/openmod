using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Building.Events
{
    public class RustPlayerUpgradingBuildingBlockEvent : RustPlayerEvent, ICancellableEvent
    {
        public BuildingBlock Block { get; }
        public ConstructionGrade Grade { get; }
        public bool IsCancelled { get; set; }
     
        public RustPlayerUpgradingBuildingBlockEvent(
            RustPlayer player,
            BuildingBlock block, 
            ConstructionGrade grade) : base(player)
        {
            Block = block;
            Grade = grade;
        }
    }
}