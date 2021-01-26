using JetBrains.Annotations;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.Rust.Oxide.Events;
using OpenMod.Rust.Players;
using OpenMod.Rust.Players.Spectating.Events;
using Oxide.Core.Plugins;

namespace OpenMod.Rust.Oxide.Players.Spectating.Events
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal class PlayerSpectatingEventsListener : OxideEventsListenerBase
    {
        public PlayerSpectatingEventsListener(IEventBus eventBus, IOpenModHost openModHost)
            : base(eventBus, openModHost)
        {
        }

        [HookMethod("OnPlayerSpectateEnd")]
        private object? OnPlayerSpectateEnd(BasePlayer player, string spectateFilter)
        {
            var @event = new RustPlayerSpectateEndingEvent(new RustPlayer(player), spectateFilter);
            return EmitCancellableReturnsObject(@event);
        }

        [HookMethod("OnPlayerSpectate")]
        private object? OnPlayerSpectate(BasePlayer player, string spectateFilter)
        {
            var @event = new RustPlayerSpectateStartingEvent(new RustPlayer(player), spectateFilter);
            return EmitCancellableReturnsObject(@event);
        }
    }
}
