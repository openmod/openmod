using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Unturned.Events;

namespace OpenMod.Unturned.Players.Equipment.Events
{
    public class UnturnedPlayerItemUnequippedEvent : UnturnedPlayerEvent, IPlayerItemUnequippedEvent
    {
        public UnturnedPlayerItemUnequippedEvent(UnturnedPlayer player) : base(player)
        {
        }
    }
}