using OpenMod.API.Eventing;

namespace OpenMod.Rust.Players.Events.BuildingBlocks
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