using OpenMod.Unturned.Entities;

namespace OpenMod.Unturned.Events.Players.Stats
{
    public class UnturnedPlayerStaminaUpdateEvent : UnturnedPlayerEvent
    {
        public byte Stamina { get; set; }

        public UnturnedPlayerStaminaUpdateEvent(UnturnedPlayer player, byte stamina) : base(player)
        {
            Stamina = stamina;
        }
    }
}
