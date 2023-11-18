using OpenMod.API.Eventing;
using OpenMod.Unturned.Events;
using SDG.Unturned;
using Steamworks;

namespace OpenMod.Unturned.Players.Quests.Events
{
    public class UnturnedPlayerJoiningGroupEvent : UnturnedPlayerEvent, ICancellableEvent
    {
        public bool IsCancelled { get; set; }

        public CSteamID NewGroupId { get; }

        public EPlayerGroupRank NewGroupRank { get; }

        public UnturnedPlayerJoiningGroupEvent(UnturnedPlayer player, CSteamID newGroupId, EPlayerGroupRank newGroupRank)
            : base(player)
        {
            NewGroupId = newGroupId;
            NewGroupRank = newGroupRank;
        }
    }
}
