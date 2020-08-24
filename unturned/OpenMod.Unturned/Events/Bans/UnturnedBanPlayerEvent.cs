using OpenMod.Core.Eventing;
using Steamworks;

namespace OpenMod.Unturned.Events.Bans
{
    public class UnturnedBanPlayerEvent : Event
    {
        public CSteamID Instigator { get; }

        public CSteamID PlayerToBan { get; }

        public uint IPToBan { get; }

        public string Reason { get; set; }

        public uint Duration { get; set; }

        public bool ShouldVanillaBan { get; set; }

        public UnturnedBanPlayerEvent(CSteamID instigator, CSteamID playerToBan, uint ipToBan, string reason, uint duration, bool shouldVanillaBan)
        {
            Instigator = instigator;
            PlayerToBan = playerToBan;
            IPToBan = ipToBan;
            Reason = reason;
            Duration = duration;
            ShouldVanillaBan = shouldVanillaBan;
        }
    }
}
