using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Events.Players.Stats
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
