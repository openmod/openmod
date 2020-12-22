using JetBrains.Annotations;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.Rust.Oxide.Events;
using OpenMod.Rust.Players;
using OpenMod.Rust.Players.Punishments.Events;
using Oxide.Core.Plugins;

namespace OpenMod.Rust.Oxide.Players.Punishments.Events
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal class PlayerPunishmentsEventsListener : OxideEventsListenerBase
    {
        public PlayerPunishmentsEventsListener(IEventBus eventBus, IOpenModHost openModHost)
            : base(eventBus, openModHost)
        {
        }

        [HookMethod("OnPlayerBanned")]
        private void OnPlayerBanned(string name, ulong id, string address, string reason)
        {
            var @event = new RustPlayerBannedEvent(name, id, address, reason);
            Emit(@event);
        }

        [HookMethod("OnPlayerKicked")]
        private void OnPlayerKicked(BasePlayer player, string reason)
        {
            var @event = new RustPlayerKickedEvent(new RustPlayer(player), reason);
            Emit(@event);
        }
    }
}
