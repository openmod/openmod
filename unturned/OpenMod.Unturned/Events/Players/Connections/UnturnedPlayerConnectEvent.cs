using OpenMod.Unturned.Entities;
using OpenMod.Unturned.Events.Players.Life;

namespace OpenMod.Unturned.Events.Players.Connections
{
    public class UnturnedPlayerConnectEvent : UnturnedPlayerSpawnEvent
    {
        public UnturnedPlayerConnectEvent(UnturnedPlayer player) : base(player)
        {
            
        }
    }
}
