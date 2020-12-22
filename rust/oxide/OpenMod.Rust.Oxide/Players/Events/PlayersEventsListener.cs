using JetBrains.Annotations;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.Rust.Oxide.Events;
using OpenMod.Rust.Players;
using OpenMod.Rust.Players.Events;
using Oxide.Core.Plugins;

namespace OpenMod.Rust.Oxide.Players.Events
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal class PlayersEventsListener : OxideEventsListenerBase
    {
        public PlayersEventsListener(IEventBus eventBus, IOpenModHost openModHost) : base(eventBus, openModHost)
        {
        }

        [HookMethod("OnPlayerInput")]
        private void OnPlayerInput(BasePlayer player, InputState input)
        {
            var @event = new RustPlayerInputReceivedEvent(new RustPlayer(player), input);
            Emit(@event);
        }
    }
}
