using JetBrains.Annotations;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.Rust.Oxide.Events;
using OpenMod.Rust.Players;
using OpenMod.Rust.Players.Connections.Events;
using Oxide.Core.Plugins;

namespace OpenMod.Rust.Oxide.Players.Connections.Events
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal class PlayerConnectionsEventsListener : OxideEventsListenerBase
    {
        public PlayerConnectionsEventsListener(IEventBus eventBus, IOpenModHost openModHost)
            : base(eventBus, openModHost)
        {
        }

        [HookMethod("OnPlayerConnected")]
        private void OnPlayerConnected(BasePlayer player)
        {
            var @event = new RustPlayerConnectedEvent(new RustPlayer(player));
            Emit(@event);
        }
    }
}
