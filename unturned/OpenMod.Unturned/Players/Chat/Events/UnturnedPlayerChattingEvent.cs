using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Unturned.Events;
using SDG.Unturned;
using UnityEngine;

namespace OpenMod.Unturned.Players.Chat.Events
{
    public class UnturnedPlayerChattingEvent : UnturnedPlayerEvent, IPlayerChattingEvent
    {
        public EChatMode Mode { get; }

        public Color Color { get; set; }

        public bool IsRich { get; set; }

        public string Message { get; }

        public bool IsCancelled { get; set; }

        public UnturnedPlayerChattingEvent(UnturnedPlayer player, EChatMode mode, Color color, bool isRich, string message) : base(player)
        {
            Mode = mode;
            Color = color;
            IsRich = isRich;
            Message = message;
        }
    }
}
