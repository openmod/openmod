using OpenMod.API.Eventing;
using OpenMod.Core.Eventing;
using Steamworks;

namespace OpenMod.Unturned.Players.Events.Bans
{
    public class UnturnedPlayerBanningEvent : Event, ICancellableEvent
    {
        public CSteamID Instigator { get; }

        public CSteamID PlayerToBan { get; }

        public uint IPToBan { get; }

        public string Reason { get; set; }

        public uint Duration { get; set; }

        public bool IsCancelled { get; set; }

        public UnturnedPlayerBanningEvent(CSteamID instigator, CSteamID playerToBan, uint ipToBan, string reason, uint duration)
        {
            Instigator = instigator;
            PlayerToBan = playerToBan;
            IPToBan = ipToBan;
            Reason = reason;
            Duration = duration;
        }
    }
}
