using JetBrains.Annotations;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.Rust.Building;
using OpenMod.Rust.Oxide.Events;
using OpenMod.Rust.Players;
using OpenMod.Rust.Players.Building.Events;
using Oxide.Core.Plugins;

namespace OpenMod.Rust.Oxide.Players.Building.Events
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal class PlayerBuildingEventsListener : OxideEventsListenerBase
    {
        public PlayerBuildingEventsListener(IEventBus eventBus, IOpenModHost openModHost) : base(eventBus, openModHost)
        {
        }

        [HookMethod("CanDemolish")]
        private bool CanDemolish(BasePlayer player, BuildingBlock block, BuildingGrade.Enum grade)
        {
            var @event = new RustPlayerDemolishingBuildingBlockEvent(new RustPlayer(player), new RustBuildingBlock(block));
            return EmitCancellableReturnsBool(@event);
        }

        [HookMethod("OnPayForPlacement")]
        private object? OnPayForPlacement(BasePlayer player, Planner planner, Construction construction)
        {
            var @event = new RustPlayerPlacingConstructionEvent(new RustPlayer(player), planner, construction);
            return EmitCancellableReturnsObject(@event);
        }

        [HookMethod("OnPayForUpgrade")]
        private object? OnPayForUpgrade(BasePlayer player, BuildingBlock block, ConstructionGrade gradeTarget)
        {
            var @event = new RustPlayerUpgradingBuildingBlockEvent(new RustPlayer(player), new RustBuildingBlock(block), gradeTarget);
            return EmitCancellableReturnsObject(@event);
        }

        [HookMethod("CanUseWires")]
        private object? CanUseWires(BasePlayer player)
        {
            var @event = new RustPlayerUsingWiresEvent(new RustPlayer(player));
            return EmitCancellableReturnsObject(@event);
        }
    }
}
