namespace OpenMod.Unturned.Players.Events.Stats
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
