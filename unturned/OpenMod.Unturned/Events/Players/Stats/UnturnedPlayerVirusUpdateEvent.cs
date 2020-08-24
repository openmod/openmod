using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Events.Players.Stats
{
    public class UnturnedPlayerVirusUpdateEvent : UnturnedPlayerEvent
    {
        public byte Virus { get; set; }

        public UnturnedPlayerVirusUpdateEvent(UnturnedPlayer player, byte virus) : base(player)
        {
            Virus = virus;
        }
    }
}
