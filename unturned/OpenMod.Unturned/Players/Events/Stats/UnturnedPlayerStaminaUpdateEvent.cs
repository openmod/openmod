namespace OpenMod.Unturned.Players.Events.Stats
{
    public class UnturnedPlayerStaminaUpdateEvent : UnturnedPlayerStatUpdateEvent
    {
        public byte Stamina { get; }

        public UnturnedPlayerStaminaUpdateEvent(UnturnedPlayer player, byte stamina) : base(player)
        {
            Stamina = stamina;
        }
    }
}
