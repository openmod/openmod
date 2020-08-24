using OpenMod.API.Eventing;
using OpenMod.Unturned.Entities;
using SDG.Unturned;
using UnityEngine;

namespace OpenMod.Unturned.Events.Chat
{
    public class UnturnedPlayerChatEvent : UnturnedPlayerEvent, ICancellableEvent
    {
        public EChatMode Mode { get; }

        public Color Color { get; set; }

        public bool IsRich { get; set; }

        public string Text { get; }

        public bool IsCancelled { get; set; }

        public UnturnedPlayerChatEvent(UnturnedPlayer player, EChatMode mode, Color color, bool isRich, string text) : base(player)
        {
            Mode = mode;
            Color = color;
            IsRich = isRich;
            Text = text;
        }
    }
}
