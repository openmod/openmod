using OpenMod.Unturned.Players.Events.Life;

namespace OpenMod.Unturned.Players.Events.Connections
{
    public class UnturnedPlayerConnectedEvent : UnturnedPlayerSpawnedEvent
    {
        public UnturnedPlayerConnectedEvent(UnturnedPlayer player) : base(player)
        {

        }
    }
}
