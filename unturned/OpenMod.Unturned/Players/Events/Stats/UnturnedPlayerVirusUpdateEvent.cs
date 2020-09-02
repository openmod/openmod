namespace OpenMod.Unturned.Players.Events.Stats
{
    public class UnturnedPlayerVirusUpdateEvent : UnturnedPlayerStatUpdateEvent
    {
        public byte Virus { get; }

        public UnturnedPlayerVirusUpdateEvent(UnturnedPlayer player, byte virus) : base(player)
        {
            Virus = virus;
        }
    }
}
