extern alias JetBrainsAnnotations;
using OpenMod.Unturned.Events;
using SDG.Unturned;
using Steamworks;

namespace OpenMod.Unturned.Players.Quests.Events
{
    public class UnturnedPlayerGroupChangedEvent : UnturnedPlayerEvent
    {
        public CSteamID OldGroupID { get; }
        public EPlayerGroupRank OldGroupRank { get;  }

        public CSteamID NewGroupID { get; }
        public EPlayerGroupRank NewGroupRank { get; }

        public UnturnedPlayerGroupChangedEvent(UnturnedPlayer player, CSteamID oldGroupID,
            EPlayerGroupRank oldGroupRank, CSteamID newGroupID, EPlayerGroupRank newGroupRank) : base(player)
        {
            OldGroupID = oldGroupID;
            OldGroupRank = oldGroupRank;
            NewGroupID = newGroupID;
        }
    }
}
