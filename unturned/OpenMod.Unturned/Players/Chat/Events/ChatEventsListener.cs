extern alias JetBrainsAnnotations;
using JetBrainsAnnotations::JetBrains.Annotations;
using OpenMod.Unturned.Events;
using SDG.Unturned;
using System;
using UnityEngine;

namespace OpenMod.Unturned.Players.Chat.Events
{
    [UsedImplicitly]
    internal class ChatEventsListener : UnturnedEventsListener
    {
        public ChatEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
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

        private void OnChatted(SteamPlayer steamPlayer, EChatMode mode, ref Color color, ref bool isRich, string text, ref bool isVisible) // lgtm [cs/too-many-ref-parameters]
        {
            var player = GetUnturnedPlayer(steamPlayer)!;

            var @event = new UnturnedPlayerChattingEvent(player, mode, color, isRich, text)
            {
                IsCancelled = !isVisible
            };

            Emit(@event);

            color = @event.Color;
            isRich = @event.IsRich;
            isVisible = !@event.IsCancelled;
        }

        private void OnServerSendingMessage(ref string text, ref Color color, SteamPlayer? nativeFromPlayer, SteamPlayer? nativeToPlayer, EChatMode mode, ref string iconURL, ref bool useRichTextFormatting) // lgtm [cs/too-many-ref-parameters]
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
