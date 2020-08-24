using OpenMod.Core.Eventing;
using SDG.Unturned;

namespace OpenMod.Unturned.Events.Environment
{
    public class UnturnedRainUpdateEvent : Event
    {
        public ELightingRain Rain { get; }

        public UnturnedRainUpdateEvent(ELightingRain rain)
        {
            Rain = rain;
        }
    }
}
