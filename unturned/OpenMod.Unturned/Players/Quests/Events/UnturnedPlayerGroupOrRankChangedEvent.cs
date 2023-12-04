using OpenMod.Unturned.Events;
using SDG.Unturned;
using Steamworks;

namespace OpenMod.Unturned.Players.Quests.Events;

public class UnturnedPlayerGroupOrRankChangedEvent : UnturnedPlayerEvent
{
    public CSteamID OldGroupId { get; }
    public EPlayerGroupRank OldGroupRank { get; }

    public CSteamID NewGroupId { get; }
    public EPlayerGroupRank NewGroupRank { get; }

    public UnturnedPlayerGroupOrRankChangedEvent(UnturnedPlayer player, CSteamID oldGroupId,
        EPlayerGroupRank oldGroupRank, CSteamID newGroupId, EPlayerGroupRank newGroupRank) : base(player)
    {
        OldGroupId = oldGroupId;
        OldGroupRank = oldGroupRank;
        NewGroupId = newGroupId;
        NewGroupRank = newGroupRank;
    }
}