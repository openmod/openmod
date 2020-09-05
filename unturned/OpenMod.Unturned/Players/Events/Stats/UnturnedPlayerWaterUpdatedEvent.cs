namespace OpenMod.Unturned.Players.Events.Stats
{
    public class UnturnedPlayerWaterUpdatedEvent : UnturnedPlayerStatUpdatedEvent
    {
        public byte Water { get; }

        public UnturnedPlayerWaterUpdatedEvent(UnturnedPlayer player, byte water) : base(player)
        {
            Water = water;
        }
    }
}
