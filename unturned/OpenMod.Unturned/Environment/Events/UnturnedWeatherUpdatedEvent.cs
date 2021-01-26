using OpenMod.Core.Eventing;
using SDG.Unturned;

namespace OpenMod.Unturned.Environment.Events
{
    /// <summary>
    /// The event that is triggered when the weather changes.
    /// </summary>
    public class UnturnedWeatherUpdatedEvent : Event
    {
        /// <value>
        /// The rain status.
        /// </value>
        public ELightingRain Rain { get; }

        /// <value>
        /// The snow status.
        /// </value>
        public ELightingSnow Snow { get; }

        public UnturnedWeatherUpdatedEvent(ELightingRain rain, ELightingSnow snow)
        {
            Rain = rain;
            Snow = snow;
        }
    }
}