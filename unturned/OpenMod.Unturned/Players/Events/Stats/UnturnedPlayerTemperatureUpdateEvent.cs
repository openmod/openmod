using SDG.Unturned;

namespace OpenMod.Unturned.Players.Events.Stats
{
    public class UnturnedPlayerTemperatureUpdateEvent : UnturnedPlayerStatUpdateEvent
    {
        public EPlayerTemperature Temperature { get; }

        public UnturnedPlayerTemperatureUpdateEvent(UnturnedPlayer player, EPlayerTemperature temperature) : base(player)
        {
            Temperature = temperature;
        }
    }
}
