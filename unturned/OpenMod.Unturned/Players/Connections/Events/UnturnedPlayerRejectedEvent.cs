using OpenMod.Core.Eventing;
using SDG.Unturned;
using Steamworks;

namespace OpenMod.Unturned.Players.Connections.Events
{
    public class UnturnedPlayerRejectedEvent : Event
    {
        public CSteamID SteamId { get; }

        public ESteamRejection Rejection { get; }

        public string Explanation { get; }

        public UnturnedPlayerRejectedEvent(CSteamID steamID, ESteamRejection rejection, string explanation)
        {
            SteamId = steamID;
            Rejection = rejection;
            Explanation = explanation;
        }
    }
}
