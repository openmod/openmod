using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Events.Players.Stats
{
    public class UnturnedPlayerWaterUpdateEvent : UnturnedPlayerStatUpdateEvent
    {
        public byte Water { get; }

        public UnturnedPlayerWaterUpdateEvent(UnturnedPlayer player, byte water) : base(player)
        {
            Water = water;
        }
    }
}
