using JetBrains.Annotations;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.Rust.Oxide.Events;
using OpenMod.Rust.Players;
using OpenMod.Rust.Players.Clothing.Events;
using Oxide.Core.Plugins;

namespace OpenMod.Rust.Oxide.Players.Clothing.Events
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal class PlayerClothingEventsListener : OxideEventsListenerBase
    {
        public PlayerClothingEventsListener(IEventBus eventBus, IOpenModHost openModHost) : base(eventBus, openModHost)
        {
        }

        [HookMethod("CanWearItem")]
        private object? CanWearItem(PlayerInventory inventory, Item item, int targetSlot)
        {
            var @event = new RustPlayerWearingClothingEvent(new RustPlayer(inventory._baseEntity), item, targetSlot);
            return EmitCancellableReturnsObject(@event);
        }
    }
}
