namespace OpenMod.Unturned.Players.Stats.Events
{
    public class UnturnedPlayerOxygenUpdatedEvent : UnturnedPlayerStatUpdatedEvent
    {
        public byte Oxygen { get; }

        public UnturnedPlayerOxygenUpdatedEvent(UnturnedPlayer player, byte oxygen) : base(player)
        {
            Oxygen = oxygen;
        }
    }
}
