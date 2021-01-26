using ConVar;
using JetBrains.Annotations;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.Rust.Oxide.Events;
using OpenMod.Rust.Players;
using OpenMod.Rust.Players.Communications.Events;
using Oxide.Core.Plugins;

namespace OpenMod.Rust.Oxide.Players.Communications.Events
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal class PlayerCommunicationsEventsListener : OxideEventsListenerBase
    {
        public PlayerCommunicationsEventsListener(IEventBus eventBus, IOpenModHost openModHost)
            : base(eventBus, openModHost)
        {
        }

        [HookMethod("OnPlayerChat")]
        private object? OnPlayerChat(BasePlayer player, string message, Chat.ChatChannel channel)
        {
            var @event = new RustPlayerChattingEvent(new RustPlayer(player), message, channel);
            return EmitCancellableReturnsObject(@event);
        }

        [HookMethod("OnPlayerVoice")]
        private object? OnPlayerVoice(BasePlayer player, byte[] data)
        {
            var @event = new RustPlayerVoiceReceivedEvent(new RustPlayer(player), data);
            return EmitCancellableReturnsObject(@event);
        }
    }
}
