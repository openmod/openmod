using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Events.Players.Stats
{
    public class UnturnedPlayerFoodUpdateEvent : UnturnedPlayerStatUpdateEvent
    {
        public byte Food { get; }

        public UnturnedPlayerFoodUpdateEvent(UnturnedPlayer player, byte food) : base(player)
        {
            Food = food;
        }
    }
}
