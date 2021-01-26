using JetBrains.Annotations;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.Rust.Oxide.Events;
using OpenMod.Rust.Players;
using OpenMod.Rust.Players.Vehicles.Events;
using Oxide.Core.Plugins;

namespace OpenMod.Rust.Oxide.Players.Vehicles.Events
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal class PlayerVehiclesEventsListener : OxideEventsListenerBase
    {
        public PlayerVehiclesEventsListener(IEventBus eventBus, IOpenModHost openModHost)
            : base(eventBus, openModHost)
        {
        }

        [HookMethod("CanPushBoat")]
        private object? CanPushBoat(BasePlayer player, MotorRowboat boat)
        {
            var @event = new RustPlayerPushingBoatEvent(new RustPlayer(player), boat);
            return EmitCancellableReturnsObject(@event);
        }
    }
}
