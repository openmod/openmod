using OpenMod.Core.Eventing;
using SDG.Unturned;

namespace OpenMod.Unturned.Environment.Events
{
    public class UnturnedWeatherUpdatedEvent : Event
    {
        public ELightingRain Rain { get; }

        public ELightingSnow Snow { get; }

        public UnturnedWeatherUpdatedEvent(ELightingRain rain, ELightingSnow snow)
        {
            Rain = rain;
            Snow = snow;
        }
    }
}