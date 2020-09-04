namespace OpenMod.Unturned.Players.Events.Stats
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
