using OpenMod.Core.Eventing;
using SDG.Unturned;
using Steamworks;

namespace OpenMod.Unturned.Players.Connections.Events
{
    public class UnturnedPlayerRejectingEvent : Event
    {
        public CSteamID SteamId { get; set; }

        public ESteamRejection Rejection { get; set; }

        public string Explanation { get; set; }

        public UnturnedPlayerRejectingEvent(CSteamID steamID, ESteamRejection rejection, string explanation)
        {
            SteamId = steamID;
            Rejection = rejection;
            Explanation = explanation;
        }
    }
}
