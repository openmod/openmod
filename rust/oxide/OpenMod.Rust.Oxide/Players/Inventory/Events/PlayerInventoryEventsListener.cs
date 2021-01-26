using JetBrains.Annotations;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.Rust.Items;
using OpenMod.Rust.Oxide.Events;
using OpenMod.Rust.Players;
using OpenMod.Rust.Players.Inventory.Events;
using Oxide.Core.Plugins;

namespace OpenMod.Rust.Oxide.Players.Inventory.Events
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal class PlayerInventoryEventsListener : OxideEventsListenerBase
    {
        public PlayerInventoryEventsListener(IEventBus eventBus, IOpenModHost openModHost) : base(eventBus, openModHost)
        {
        }

        [HookMethod("OnActiveItemChanged")]
        private void OnActiveItemChanged(BasePlayer player, Item oldItem, Item newItem)
        {
            var @event = new RustPlayerActiveItemChangedEvent(
                new RustPlayer(player), new RustItem(oldItem), new RustItem(newItem));
            Emit(@event);
        }

        [HookMethod("OnPlayerDropActiveItem")]
        private object? OnPlayerDropActiveItem(BasePlayer player, Item item)
        {
            var @event = new RustPlayerActiveItemDroppedEvent(new RustPlayer(player), new RustItem(item));
            return EmitCancellableReturnsObject(@event);
        }

        [HookMethod("OnItemCraft")]
        private object? OnItemCraft(ItemCraftTask task, BasePlayer player, Item item)
        {
            var @event = new RustPlayerCraftingItemEvent(new RustPlayer(player), task.blueprint, task.amount)
            {
                IsCancelled = task.cancelled
            };
            Emit(@event);
            return @event.IsCancelled ? (object) false : null;
        }

        [HookMethod("OnExperimentStart")]
        private object? OnExperimentStart(Workbench workbench, BasePlayer player)
        {
            var @event = new RustPlayerExperimentingEvent(new RustPlayer(player), workbench);
            return EmitCancellableReturnsObject(@event);
        }

        [HookMethod("OnItemPickup")]
        private object? OnItemPickup(Item item, BasePlayer player)
        {
            var @event = new RustPlayerPickingUpItemEvent(new RustPlayer(player), new RustItem(item), item.position);
            return EmitCancellableReturnsObject(@event);
        }

        [HookMethod("CanPickupLock")]
        private object? CanPickupLock(BasePlayer player, BaseLock baseLock)
        {
            var @event = new RustPlayerPickingUpLockEvent(new RustPlayer(player), baseLock);
            return EmitCancellableReturnsObject(@event);
        }

        [HookMethod("CanResearchItem")]
        private object? CanResearchItem(BasePlayer player, Item targetItem)
        {
            var @event = new RustPlayerResearchingItemEvent(new RustPlayer(player), new RustItem(targetItem));
            return EmitCancellableReturnsObject(@event);
        }
    }
}
