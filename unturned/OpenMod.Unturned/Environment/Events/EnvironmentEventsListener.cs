using OpenMod.API;
using OpenMod.Unturned.Events;
using SDG.Unturned;
using System;

namespace OpenMod.Unturned.Environment.Events
{
    [OpenModInternal]
    internal class EnvironmentEventsListener : UnturnedEventsListener
    {
        public EnvironmentEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        public override void Subscribe()
        {
            LightingManager.onDayNightUpdated += OnDayNightUpdated;
            LightingManager.onRainUpdated += OnRainUpdated;
            LightingManager.onSnowUpdated += OnSnowUpdated;
        }

        public override void Unsubscribe()
        {
            LightingManager.onDayNightUpdated -= OnDayNightUpdated;
            LightingManager.onRainUpdated -= OnRainUpdated;
            LightingManager.onSnowUpdated -= OnSnowUpdated;
        }

        private void OnDayNightUpdated(bool isDaytime)
        {
            var worldTime = isDaytime ? WorldTime.Day : WorldTime.Night;

            var @event = new UnturnedDayNightUpdatedEvent(worldTime, LightingManager.isFullMoon);

            Emit(@event);
        }

        private void OnRainUpdated(ELightingRain rain)
        {
            var @event = new UnturnedWeatherUpdatedEvent(rain, LevelLighting.snowyness);

            Emit(@event);
        }

        private void OnSnowUpdated(ELightingSnow snow)
        {
            var @event = new UnturnedWeatherUpdatedEvent(LevelLighting.rainyness, snow);

            Emit(@event);
        }
    }
}
