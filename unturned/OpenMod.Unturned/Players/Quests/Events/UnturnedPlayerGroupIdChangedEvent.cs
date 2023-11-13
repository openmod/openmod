using OpenMod.Unturned.Events;
using Steamworks;

namespace OpenMod.Unturned.Players.Quests.Events;

public class UnturnedPlayerGroupIdChangedEvent : UnturnedPlayerEvent
{
    public CSteamID OldGroupId { get; }

    public CSteamID NewGroupId { get; }

    public UnturnedPlayerGroupIdChangedEvent(UnturnedPlayer player, CSteamID oldGroupId,
        CSteamID newGroupId) : base(player)
    {
        OldGroupId = oldGroupId;
        NewGroupId = newGroupId;
    }
}