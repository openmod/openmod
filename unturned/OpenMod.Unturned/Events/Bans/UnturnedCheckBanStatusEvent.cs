using OpenMod.Core.Eventing;
using SDG.Unturned;

namespace OpenMod.Unturned.Events.Bans
{
    public class UnturnedCheckBanStatusEvent : Event
    {
        public SteamPlayerID SteamPlayerId { get; }
        
        public uint RemoteIP { get; }

        public bool IsBanned { get; set; }

        public string Reason { get; set; }

        public uint RemainingDuration { get; set; }

        public UnturnedCheckBanStatusEvent(SteamPlayerID steamPlayerId, uint remoteIp, bool isBanned, string reason, uint remainingDuration)
        {
            SteamPlayerId = steamPlayerId;
            RemoteIP = remoteIp;
            IsBanned = isBanned;
            Reason = reason;
            RemainingDuration = remainingDuration;
        }
    }
}
