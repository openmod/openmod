using OpenMod.Core.Eventing;
using Steamworks;

namespace OpenMod.Unturned.Players.Bans.Events
{
    public class UnturnedPlayerBannedEvent : Event
    {
        public CSteamID Instigator { get; }

        public CSteamID BannedPlayer { get; }

        public uint IP { get; }

        public string Reason { get; set; }

        public uint Duration { get; set; }

        public UnturnedPlayerBannedEvent(CSteamID instigator, CSteamID bannedPlayer, uint ip, string reason, uint duration)
        {
            Instigator = instigator;
            BannedPlayer = bannedPlayer;
            IP = ip;
            Reason = reason;
            Duration = duration;
        }
    }
}
