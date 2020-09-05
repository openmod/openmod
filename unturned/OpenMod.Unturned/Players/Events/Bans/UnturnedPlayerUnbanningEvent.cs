using OpenMod.Core.Eventing;
using Steamworks;

namespace OpenMod.Unturned.Players.Events.Bans
{
    public class UnturnedPlayerUnbanningEvent : Event
    {
        public CSteamID Instigator { get; }

        public CSteamID PlayerToUnban { get; }

        public bool ShouldVanillaUnban { get; set; }

        public UnturnedPlayerUnbanningEvent(CSteamID instigator, CSteamID playerToUnban, bool shouldVanillaUnban)
        {
            Instigator = instigator;
            PlayerToUnban = playerToUnban;
            ShouldVanillaUnban = shouldVanillaUnban;
        }
    }
}
