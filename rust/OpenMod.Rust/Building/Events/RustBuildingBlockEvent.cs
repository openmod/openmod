using OpenMod.Core.Eventing;
using OpenMod.Extensions.Games.Abstractions.Building;

namespace OpenMod.Rust.Building.Events
{
    public class RustBuildingBlockEvent : Event, IBuildableEvent
    {
        public RustBuildingBlock BuildingBlock { get; }

        public RustBuildingBlockEvent(RustBuildingBlock buildingBlock)
        {
            BuildingBlock = buildingBlock;
        }

        public IBuildable Buildable => BuildingBlock;
    }
}