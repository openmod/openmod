namespace OpenMod.Unturned.Players.Events.Stats
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
