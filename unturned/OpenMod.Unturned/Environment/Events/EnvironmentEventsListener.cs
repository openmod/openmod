using HarmonyLib;
using OpenMod.API;
using OpenMod.Unturned.Events;
using OpenMod.Unturned.Patching;
using SDG.Unturned;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

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

        [HarmonyPatch(typeof(LightingManager), "onPrePreLevelLoaded")]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private static class Patch_LightingManager
        {
            [HarmonyCleanup]
            public static Exception? Cleanup(Exception ex, MethodBase original)
            {
                HarmonyExceptionHandler.ReportCleanupException(typeof(Patch_LightingManager), ex, original);
                return null;
            }

            [HarmonyPrefix]
            public static bool OnPrePreLevelLoaded()
            {
                return false;
            }
        }
    }
}
