namespace OpenMod.Unturned.Players.Stats.Events
{
    public class UnturnedPlayerFoodUpdatedEvent : UnturnedPlayerStatUpdatedEvent
    {
        public byte Food { get; }

        public UnturnedPlayerFoodUpdatedEvent(UnturnedPlayer player, byte food) : base(player)
        {
            Food = food;
        }
    }
}
