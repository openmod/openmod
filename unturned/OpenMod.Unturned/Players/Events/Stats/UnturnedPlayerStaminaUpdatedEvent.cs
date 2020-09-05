namespace OpenMod.Unturned.Players.Events.Stats
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
