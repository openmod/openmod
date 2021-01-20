using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.Unturned.Events;
using SDG.Unturned;
using UnityEngine;
// ReSharper disable DelegateSubtraction

namespace OpenMod.Unturned.Players.Chat.Events
{
    internal class ChatEventsListener : UnturnedEventsListener
    {
        public ChatEventsListener(IOpenModHost openModHost,
            IEventBus eventBus,
            IUserManager userManager) : base(openModHost, eventBus, userManager)
        {
        }

        public override void Subscribe()
        {
            ChatManager.onChatted += OnChatted;
            ChatManager.onServerSendingMessage += OnServerSendingMessage;
        }

        public override void Unsubscribe()
        {
            ChatManager.onChatted -= OnChatted;
            ChatManager.onServerSendingMessage -= OnServerSendingMessage;
        }

        // lgtm [cs/too-many-ref-parameters]
        private void OnChatted(SteamPlayer steamPlayer, EChatMode mode, ref Color color, ref bool isRich, string text, ref bool isVisible)
        {
            var player = GetUnturnedPlayer(steamPlayer);

            var @event = new UnturnedPlayerChattingEvent(player, mode, color, isRich, text)
            {
                IsCancelled = !isVisible
            };

            Emit(@event);

            color = @event.Color;
            isRich = @event.IsRich;
            isVisible = !@event.IsCancelled;
        }

        // lgtm [cs/too-many-ref-parameters]
        private void OnServerSendingMessage(ref string text, ref Color color, SteamPlayer nativeFromPlayer, SteamPlayer nativeToPlayer, EChatMode mode, ref string iconURL, ref bool useRichTextFormatting)
        {
            // If nativeToPlayer is null, this event will be called again for each player
            if (nativeToPlayer == null) return;

            var fromPlayer = GetUnturnedPlayer(nativeFromPlayer);
            var toPlayer = GetUnturnedPlayer(nativeToPlayer);

            var @event = new UnturnedServerSendingMessageEvent(fromPlayer, toPlayer, text, color, mode, iconURL, useRichTextFormatting);

            Emit(@event);

            text = @event.Text;
            color = @event.Color;
            iconURL = @event.IconUrl;
            useRichTextFormatting = @event.IsRich;
        }
    }
}
