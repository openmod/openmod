using SDG.Unturned;

namespace OpenMod.Unturned.Players.Stats.Events
{
    public class UnturnedPlayerStatIncrementedEvent : UnturnedPlayerStatUpdatedEvent
    {
        public UnturnedPlayerStatIncrementedEvent(UnturnedPlayer player, EPlayerStat stat) : base(player, stat)
        {

        }
    }
}
