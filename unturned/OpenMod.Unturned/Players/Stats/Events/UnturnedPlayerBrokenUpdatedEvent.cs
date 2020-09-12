namespace OpenMod.Unturned.Players.Stats.Events
{
    public class UnturnedPlayerBrokenUpdatedEvent : UnturnedPlayerStatUpdatedEvent
    {
        public bool IsBroken { get; }

        public UnturnedPlayerBrokenUpdatedEvent(UnturnedPlayer player, bool isBroken) : base(player)
        {
            IsBroken = isBroken;
        }
    }
}
