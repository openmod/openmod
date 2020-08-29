using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Unturned.Entities;
using SDG.Unturned;
using UnityEngine;

namespace OpenMod.Unturned.Events.Chat
{
    public class UnturnedPlayerChatEvent : UnturnedPlayerEvent, IPlayerChatEvent
    {
        public EChatMode Mode { get; }

        public Color Color { get; set; }

        public bool IsRich { get; set; }

        public string Message { get; }

        public bool IsCancelled { get; set; }

        public UnturnedPlayerChatEvent(UnturnedPlayer player, EChatMode mode, Color color, bool isRich, string message) : base(player)
        {
            Mode = mode;
            Color = color;
            IsRich = isRich;
            Message = message;
        }
    }
}
