namespace OpenMod.Unturned.Players.Events.Stats
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
