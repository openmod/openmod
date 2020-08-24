using OpenMod.Unturned.Entities;
using SDG.Unturned;

namespace OpenMod.Unturned.Events.Players.Stats
{
    public class UnturnedPlayerTemperatureUpdateEvent : UnturnedPlayerEvent
    {
        public EPlayerTemperature Temperature { get; set; }

        public UnturnedPlayerTemperatureUpdateEvent(UnturnedPlayer player, EPlayerTemperature temperature) : base(player)
        {
            Temperature = temperature;
        }
    }
}
