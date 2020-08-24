using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Events.Players.Stats
{
    public class UnturnedPlayerBleedingUpdateEvent : UnturnedPlayerEvent
    {
        public bool IsBleeding { get; set; }

        public UnturnedPlayerBleedingUpdateEvent(UnturnedPlayer player, bool isBleeding) : base(player)
        {
            IsBleeding = isBleeding;
        }
    }
}
