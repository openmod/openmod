namespace OpenMod.Unturned.Players.Stats.Events
{
    public class UnturnedPlayerHealthUpdatedEvent : UnturnedPlayerStatUpdatedEvent
    {
        public byte Health { get; }

        public UnturnedPlayerHealthUpdatedEvent(UnturnedPlayer player, byte health) : base(player)
        {
            Health = health;
        }
    }
}
