using OpenMod.Unturned.Events;
using SDG.Unturned;

namespace OpenMod.Unturned.Players.Stats.Events
{
    public abstract class UnturnedPlayerStatUpdatedEvent : UnturnedPlayerEvent
    {
        public EPlayerStat Stat { get; }

        protected UnturnedPlayerStatUpdatedEvent(UnturnedPlayer player, EPlayerStat stat) : base(player)
        {
            Stat = stat;
        }
    }
}
