using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Unturned.Events;

namespace OpenMod.Unturned.Players.Life.Events
{
    public class UnturnedPlayerSpawnedEvent : UnturnedPlayerEvent, IPlayerSpawnedEvent
    {
        protected UnturnedPlayerSpawnedEvent(UnturnedPlayer player) : base(player)
        {

        }
    }
}
