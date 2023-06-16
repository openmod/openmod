using OpenMod.Unturned.Events;

namespace OpenMod.Unturned.Players.BattlEye.Events;

public class UnturnedBattlEyeKickedEvent : UnturnedPlayerEvent
{
    public string Reason { get; }

    public UnturnedBattlEyeKickedEvent(UnturnedPlayer player, string reason) : base(player)
    {
        Reason = reason;
    }
}
