using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenMod.Unturned.Events.Environment
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
            LightingManager.onMoonUpdated += OnMoonUpdated;
            LightingManager.onRainUpdated += OnRainUpdated;
            LightingManager.onSnowUpdated += OnSnowUpdated;
        }

        public override void Unsubscribe()
        {
            LightingManager.onDayNightUpdated -= OnDayNightUpdated;
            LightingManager.onMoonUpdated -= OnMoonUpdated;
            LightingManager.onRainUpdated -= OnRainUpdated;
            LightingManager.onSnowUpdated -= OnSnowUpdated;
        }

        private void OnDayNightUpdated(bool isDaytime)
        {
            UnturnedDayNightUpdateEvent @event = new UnturnedDayNightUpdateEvent(isDaytime);

            Emit(@event);
        }

        private void OnRainUpdated(ELightingRain rain)
        {
            UnturnedRainUpdateEvent @event = new UnturnedRainUpdateEvent(rain);

            Emit(@event);
        }

        private void OnMoonUpdated(bool isFullMoon)
        {
            UnturnedMoonUpdateEvent @event = new UnturnedMoonUpdateEvent(isFullMoon);

            Emit(@event);
        }

        private void OnSnowUpdated(ELightingSnow snow)
        {
            UnturnedSnowUpdateEvent @event = new UnturnedSnowUpdateEvent(snow);

            Emit(@event);
        }
    }
}
