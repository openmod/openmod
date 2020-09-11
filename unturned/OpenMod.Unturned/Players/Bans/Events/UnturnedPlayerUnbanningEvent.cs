using OpenMod.API.Eventing;
using OpenMod.Core.Eventing;
using Steamworks;

namespace OpenMod.Unturned.Players.Bans.Events
{
    public class UnturnedPlayerUnbanningEvent : Event, ICancellableEvent
    {
        public CSteamID Instigator { get; }

        public CSteamID PlayerToUnban { get; }

        public bool IsCancelled { get; set; }

        public UnturnedPlayerUnbanningEvent(CSteamID instigator, CSteamID playerToUnban)
        {
            Instigator = instigator;
            PlayerToUnban = playerToUnban;
        }
    }
}