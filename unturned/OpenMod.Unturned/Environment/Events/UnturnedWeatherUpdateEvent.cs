using OpenMod.Core.Eventing;
using SDG.Unturned;

namespace OpenMod.Unturned.Environment.Events
{
    public class UnturnedWeatherUpdateEvent : Event
    {
        public ELightingRain Rain { get; }

        public ELightingSnow Snow { get; }

        public UnturnedWeatherUpdateEvent(ELightingRain rain, ELightingSnow snow)
        {
            Rain = rain;
            Snow = snow;
        }
    }
}