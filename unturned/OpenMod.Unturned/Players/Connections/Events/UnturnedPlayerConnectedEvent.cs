using OpenMod.Unturned.Players.Life.Events;

namespace OpenMod.Unturned.Players.Connections.Events
{
    public class UnturnedPlayerConnectedEvent : UnturnedPlayerSpawnedEvent
    {
        public UnturnedPlayerConnectedEvent(UnturnedPlayer player) : base(player)
        {

        }
    }
}
