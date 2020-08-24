using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Events.Players.Stats
{
    public class UnturnedPlayerOxygenUpdateEvent : UnturnedPlayerEvent
    {
        public byte Oxygen { get; set; }

        public UnturnedPlayerOxygenUpdateEvent(UnturnedPlayer player, byte oxygen) : base(player)
        {
            Oxygen = oxygen;
        }
    }
}
