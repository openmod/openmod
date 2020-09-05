namespace OpenMod.Unturned.Players.Events.Stats
{
    public class UnturnedPlayerBleedingUpdatedEvent : UnturnedPlayerStatUpdatedEvent
    {
        public bool IsBleeding { get; }

        public UnturnedPlayerBleedingUpdatedEvent(UnturnedPlayer player, bool isBleeding) : base(player)
        {
            IsBleeding = isBleeding;
        }
    }
}
