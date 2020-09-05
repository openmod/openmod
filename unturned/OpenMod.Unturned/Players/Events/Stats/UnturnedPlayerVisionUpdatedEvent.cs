namespace OpenMod.Unturned.Players.Events.Stats
{
    public class UnturnedPlayerVisionUpdatedEvent : UnturnedPlayerStatUpdatedEvent
    {
        public bool IsViewing { get; }

        public UnturnedPlayerVisionUpdatedEvent(UnturnedPlayer player, bool isViewing) : base(player)
        {
            IsViewing = isViewing;
        }
    }
}
