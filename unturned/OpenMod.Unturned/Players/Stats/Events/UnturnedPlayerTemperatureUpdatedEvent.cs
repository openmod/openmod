using SDG.Unturned;

namespace OpenMod.Unturned.Players.Stats.Events
{
    public class UnturnedPlayerTemperatureUpdatedEvent : UnturnedPlayerStatUpdatedEvent
    {
        public EPlayerTemperature Temperature { get; }

        public UnturnedPlayerTemperatureUpdatedEvent(UnturnedPlayer player, EPlayerTemperature temperature) : base(player)
        {
            Temperature = temperature;
        }
    }
}
