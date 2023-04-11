using OpenMod.Unturned.Events;

namespace OpenMod.Unturned.Players.BattleEye.Events
{
    public class UnturnedBattleEyeKickedEvent : UnturnedPlayerEvent
    {
        public string Reason { get; set; }

        public UnturnedBattleEyeKickedEvent(UnturnedPlayer player, string reason) : base(player)
        {
            Reason = reason;
        }
    }
}
