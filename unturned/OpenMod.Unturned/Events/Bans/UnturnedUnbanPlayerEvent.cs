using OpenMod.Core.Eventing;
using Steamworks;

namespace OpenMod.Unturned.Events.Bans
{
    public class UnturnedUnbanPlayerEvent : Event
    {
        public CSteamID Instigator { get; }

        public CSteamID PlayerToUnban { get; }

        public bool ShouldVanillaUnban { get; set; }

        public UnturnedUnbanPlayerEvent(CSteamID instigator, CSteamID playerToUnban, bool shouldVanillaUnban)
        {
            Instigator = instigator;
            PlayerToUnban = playerToUnban;
            ShouldVanillaUnban = shouldVanillaUnban;
        }
    }
}
