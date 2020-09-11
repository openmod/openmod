using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Unturned.Events;

namespace OpenMod.Unturned.Players.Events.Equipment
{
    public abstract class UnturnedPlayerItemUnequippedEvent : UnturnedPlayerEvent, IPlayerItemUnequippedEvent
    {
        protected UnturnedPlayerItemUnequippedEvent(UnturnedPlayer player) : base(player)
        {
        }
    }
}