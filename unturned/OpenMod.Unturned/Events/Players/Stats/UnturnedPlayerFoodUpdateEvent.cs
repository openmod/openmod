using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Events.Players.Stats
{
    public class UnturnedPlayerFoodUpdateEvent : UnturnedPlayerEvent
    {
        public byte Food { get; set; }

        public UnturnedPlayerFoodUpdateEvent(UnturnedPlayer player, byte food) : base(player)
        {
            Food = food;
        }
    }
}
