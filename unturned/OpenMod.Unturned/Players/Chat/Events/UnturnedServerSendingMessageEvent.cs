using SDG.Unturned;
using UnityEngine;
using Event = OpenMod.Core.Eventing.Event;

namespace OpenMod.Unturned.Players.Chat.Events
{
    public class UnturnedServerSendingMessageEvent : Event
    {
        public UnturnedPlayer? FromPlayer { get; }

        public UnturnedPlayer? ToPlayer { get; }

        public string Text { get; set; }

        public Color Color { get; set; }

        public EChatMode Mode { get; }

        public string IconUrl { get; set; }

        public bool IsRich { get; set; }

        public UnturnedServerSendingMessageEvent(UnturnedPlayer? fromPlayer, UnturnedPlayer? toPlayer, string text, Color color, EChatMode mode, string iconUrl, bool isRich)
        {
            FromPlayer = fromPlayer;
            ToPlayer = toPlayer;
            Text = text;
            Color = color;
            Mode = mode;
            IconUrl = iconUrl;
            IsRich = isRich;
        }
    }
}
