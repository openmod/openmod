using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Events.Players.Life
{
    public class UnturnedPlayerSpawnEvent : UnturnedPlayerEvent, IPlayerSpawnEvent
    {
        protected UnturnedPlayerSpawnEvent(UnturnedPlayer player) : base(player)
        {

        }
    }
}
