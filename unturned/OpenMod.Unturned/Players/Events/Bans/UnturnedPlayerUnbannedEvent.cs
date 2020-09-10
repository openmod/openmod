using OpenMod.Core.Eventing;
using Steamworks;

namespace OpenMod.Unturned.Players.Events.Bans
{
    public class UnturnedPlayerUnbannedEvent : Event
    {
        public CSteamID UnbannedPlayer { get; }

        public UnturnedPlayerUnbannedEvent(CSteamID unbannedPlayer)
        {
            UnbannedPlayer = unbannedPlayer;
        }
    }
}