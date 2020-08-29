using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Events.Players.Stats
{
    public class UnturnedPlayerOxygenUpdateEvent : UnturnedPlayerStatUpdateEvent
    {
        public byte Oxygen { get; }

        public UnturnedPlayerOxygenUpdateEvent(UnturnedPlayer player, byte oxygen) : base(player)
        {
            Oxygen = oxygen;
        }
    }
}
