using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Unturned.Events;

namespace OpenMod.Unturned.Players.Events.Equipment
{
    public abstract class UnturnedPlayerUnequippedEvent : UnturnedPlayerEvent, IPlayerItemUnequippedEvent
    {
        protected UnturnedPlayerUnequippedEvent(UnturnedPlayer player) : base(player)
        {
        }
    }
}