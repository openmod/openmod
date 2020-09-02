using OpenMod.Core.Eventing;
using Steamworks;

namespace OpenMod.Unturned.Players.Events.Bans
{
    public class UnturnedPlayerUnbanEvent : Event
    {
        public CSteamID Instigator { get; }

        public CSteamID PlayerToUnban { get; }

        public bool ShouldVanillaUnban { get; set; }

        public UnturnedPlayerUnbanEvent(CSteamID instigator, CSteamID playerToUnban, bool shouldVanillaUnban)
        {
            Instigator = instigator;
            PlayerToUnban = playerToUnban;
            ShouldVanillaUnban = shouldVanillaUnban;
        }
    }
}
