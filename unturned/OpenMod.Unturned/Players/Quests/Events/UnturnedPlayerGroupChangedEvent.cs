using System;
using SDG.Unturned;
using Steamworks;

namespace OpenMod.Unturned.Players.Quests.Events
{
    [Obsolete($"Use {nameof(UnturnedPlayerGroupIdChangedEvent)} or {nameof(UnturnedPlayerGroupOrRankChangedEvent)}")]
    public class UnturnedPlayerGroupChangedEvent : UnturnedPlayerGroupOrRankChangedEvent
    {
        public UnturnedPlayerGroupChangedEvent(UnturnedPlayer player, CSteamID oldGroupId, EPlayerGroupRank oldGroupRank, CSteamID newGroupId, EPlayerGroupRank newGroupRank) : base(player, oldGroupId, oldGroupRank, newGroupId, newGroupRank)
        { }
    }
}
