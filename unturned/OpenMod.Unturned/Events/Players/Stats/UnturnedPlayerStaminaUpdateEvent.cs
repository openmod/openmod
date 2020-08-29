using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Events.Players.Stats
{
    public class UnturnedPlayerStaminaUpdateEvent : UnturnedPlayerStatUpdateEvent
    {
        public byte Stamina { get; }

        public UnturnedPlayerStaminaUpdateEvent(UnturnedPlayer player, byte stamina) : base(player)
        {
            Stamina = stamina;
        }
    }
}
