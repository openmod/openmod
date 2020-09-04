using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.Unturned.Events;
using SDG.Unturned;

namespace OpenMod.Unturned.Environment.Events
{
    internal class EnvironmentEventsListener : UnturnedEventsListener
    {
        public EnvironmentEventsListener(IOpenModHost openModHost,
            IEventBus eventBus,
            IUserManager userManager) : base(openModHost, eventBus, userManager)
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
            WorldTime worldTime = isDaytime ? WorldTime.Day : WorldTime.Night;

            UnturnedDayNightUpdatedEvent @event = new UnturnedDayNightUpdatedEvent(worldTime, LightingManager.isFullMoon);

            Emit(@event);
        }

        private void OnRainUpdated(ELightingRain rain)
        {
            UnturnedWeatherUpdatedEvent @event = new UnturnedWeatherUpdatedEvent(rain, LevelLighting.snowyness);

            Emit(@event);
        }

        private void OnSnowUpdated(ELightingSnow snow)
        {
            UnturnedWeatherUpdatedEvent @event = new UnturnedWeatherUpdatedEvent(LevelLighting.rainyness, snow);

            Emit(@event);
        }
    }
}
