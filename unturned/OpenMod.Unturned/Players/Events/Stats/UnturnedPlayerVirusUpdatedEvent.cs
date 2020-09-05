namespace OpenMod.Unturned.Players.Events.Stats
{
    public class UnturnedPlayerVirusUpdatedEvent : UnturnedPlayerStatUpdatedEvent
    {
        public byte Virus { get; }

        public UnturnedPlayerVirusUpdatedEvent(UnturnedPlayer player, byte virus) : base(player)
        {
            Virus = virus;
        }
    }
}
