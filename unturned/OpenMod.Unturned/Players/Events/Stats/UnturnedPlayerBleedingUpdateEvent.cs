using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Players.Events.Stats
{
    public class UnturnedPlayerBleedingUpdateEvent : UnturnedPlayerStatUpdateEvent
    {
        public bool IsBleeding { get; }

        public UnturnedPlayerBleedingUpdateEvent(UnturnedPlayer player, bool isBleeding) : base(player)
        {
            IsBleeding = isBleeding;
        }
    }
}
