using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Events.Players.Stats
{
    public class UnturnedPlayerWaterUpdateEvent : UnturnedPlayerEvent
    {
        public byte Water { get; set; }

        public UnturnedPlayerWaterUpdateEvent(UnturnedPlayer player, byte water) : base(player)
        {
            Water = water;
        }
    }
}
