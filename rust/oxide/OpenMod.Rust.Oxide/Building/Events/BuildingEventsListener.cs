using JetBrains.Annotations;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.Rust.Building;
using OpenMod.Rust.Building.Events;
using OpenMod.Rust.Oxide.Events;
using OpenMod.Rust.Players;
using Oxide.Core.Plugins;

namespace OpenMod.Rust.Oxide.Building.Events
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal class BuildingEventsListener : OxideEventsListenerBase
    {
        public BuildingEventsListener(IEventBus eventBus, IOpenModHost openModHost) : base(eventBus, openModHost)
        {
        }

        [HookMethod("CanChangeGrade")]
        private bool CanChangeGrade(BasePlayer player, BuildingBlock block, BuildingGrade.Enum grade)
        {
            var @event = new RustBuildingBlockUpgradingEvent(new RustBuildingBlock(block), new RustPlayer(player), grade);
            return EmitCancellableReturnsBool(@event);
        }
    }
}
