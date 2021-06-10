using OpenMod.Unturned.Events;
using SDG.Unturned;

namespace OpenMod.Unturned.Players.Quests.Events
{
    public class UnturnedPlayerGroupRankChangedEvent : UnturnedPlayerEvent
    {
        public EPlayerGroupRank OldGroupRank { get;  }

        public EPlayerGroupRank NewGroupRank { get; }

        public UnturnedPlayerGroupRankChangedEvent(UnturnedPlayer player, EPlayerGroupRank oldGroupRank,
            EPlayerGroupRank newGroupRank) : base(player)
        {
            OldGroupRank = oldGroupRank;
            NewGroupRank = newGroupRank;
        }
    }
}
