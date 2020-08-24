using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Events.Players.Stats
{
    public class UnturnedPlayerHealthUpdateEvent : UnturnedPlayerEvent
    {
        public byte Health { get; set; }

        public UnturnedPlayerHealthUpdateEvent(UnturnedPlayer player, byte health) : base(player)
        {
            Health = health;
        }
    }
}
