namespace OpenMod.Unturned.Players.Events.Stats
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
