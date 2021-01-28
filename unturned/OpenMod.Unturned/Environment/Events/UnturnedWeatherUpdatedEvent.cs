using OpenMod.Core.Eventing;
using SDG.Unturned;

namespace OpenMod.Unturned.Environment.Events
{
    /// <summary>
    /// The event that is triggered when the weather changes.
    /// </summary>
    public class UnturnedWeatherUpdatedEvent : Event
    {
        /// <summary>
        /// Gets the rain status.
        /// </summary>
        public ELightingRain Rain { get; }

        /// <summary>
        /// Gets the snow status.
        /// </summary>
        public ELightingSnow Snow { get; }

        public UnturnedWeatherUpdatedEvent(ELightingRain rain, ELightingSnow snow)
        {
            Rain = rain;
            Snow = snow;
        }
    }
}