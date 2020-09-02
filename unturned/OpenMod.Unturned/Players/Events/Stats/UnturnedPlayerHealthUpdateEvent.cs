namespace OpenMod.Unturned.Players.Events.Stats
{
    public class UnturnedPlayerHealthUpdateEvent : UnturnedPlayerStatUpdateEvent
    {
        public byte Health { get; }

        public UnturnedPlayerHealthUpdateEvent(UnturnedPlayer player, byte health) : base(player)
        {
            Health = health;
        }
    }
}
