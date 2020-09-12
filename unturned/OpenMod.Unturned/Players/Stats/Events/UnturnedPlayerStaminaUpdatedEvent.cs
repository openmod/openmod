namespace OpenMod.Unturned.Players.Stats.Events
{
    public class UnturnedPlayerStaminaUpdatedEvent : UnturnedPlayerStatUpdatedEvent
    {
        public byte Stamina { get; }

        public UnturnedPlayerStaminaUpdatedEvent(UnturnedPlayer player, byte stamina) : base(player)
        {
            Stamina = stamina;
        }
    }
}
