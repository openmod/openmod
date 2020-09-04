using HarmonyLib;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.Unturned.Events;
using SDG.Unturned;

namespace OpenMod.Unturned.Players.Events.Stats
{
    internal class PlayerStatsEventsListener : UnturnedPlayerEventsListener
    {
        public PlayerStatsEventsListener(IOpenModHost openModHost,
            IEventBus eventBus,
            IUserManager userManager) : base(openModHost, eventBus, userManager)
        {

        }

        public override void Subscribe()
        {
            OnBleedingUpdated += EventsOnBleedingUpdated;
            OnBrokenUpdated += EventsOnBrokenUpdated;
            OnFoodUpdated += EventsOnFoodUpdated;
            OnHealthUpdated += EventsOnHealthUpdated;
            OnVirusUpdated += EventsOnVirusUpdated;
            OnWaterUpdated += EventsOnWaterUpdated;
        }

        public override void Unsubscribe()
        {
            OnBleedingUpdated -= EventsOnBleedingUpdated;
            OnBrokenUpdated -= EventsOnBrokenUpdated;
            OnFoodUpdated -= EventsOnFoodUpdated;
            OnHealthUpdated -= EventsOnHealthUpdated;
            OnVirusUpdated -= EventsOnVirusUpdated;
            OnWaterUpdated -= EventsOnWaterUpdated;
        }

        public override void SubscribePlayer(Player player)
        {
            player.life.onLifeUpdated += isDead => OnLifeUpdated(player, isDead);
            player.life.onOxygenUpdated += oxygen => OnOxygenUpdated(player, oxygen);
            player.life.onStaminaUpdated += stamina => OnStaminaUpdated(player, stamina);
            player.life.onTemperatureUpdated += temperature => OnTemperatureUpdated(player, temperature);
            player.life.onVirusUpdated += virus => EventsOnVirusUpdated(player, virus);
            player.life.onVisionUpdated += viewing => OnVisionUpdated(player, viewing);
        }

        public override void UnsubscribePlayer(Player player)
        {
            player.life.onLifeUpdated -= isDead => OnLifeUpdated(player, isDead);
            player.life.onOxygenUpdated -= oxygen => OnOxygenUpdated(player, oxygen);
            player.life.onStaminaUpdated -= stamina => OnStaminaUpdated(player, stamina);
            player.life.onTemperatureUpdated -= temperature => OnTemperatureUpdated(player, temperature);
            player.life.onVirusUpdated -= virus => EventsOnVirusUpdated(player, virus);
            player.life.onVisionUpdated -= viewing => OnVisionUpdated(player, viewing);
        }

        private void EventsOnBleedingUpdated(Player nativePlayer, bool isBleeding)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerBleedingUpdatedEvent @event = new UnturnedPlayerBleedingUpdatedEvent(player, isBleeding);

            Emit(@event);
        }

        private void EventsOnBrokenUpdated(Player nativePlayer, bool isBroken)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerBrokenUpdatedEvent @event = new UnturnedPlayerBrokenUpdatedEvent(player, isBroken);

            Emit(@event);
        }

        private void EventsOnFoodUpdated(Player nativePlayer, byte food)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerFoodUpdatedEvent @event = new UnturnedPlayerFoodUpdatedEvent(player, food);

            Emit(@event);
        }

        private void EventsOnHealthUpdated(Player nativePlayer, byte health)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerHealthUpdatedEvent @event = new UnturnedPlayerHealthUpdatedEvent(player, health);

            Emit(@event);
        }

        private void OnLifeUpdated(Player nativePlayer, bool isDead)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerLifeUpdatedEvent @event = new UnturnedPlayerLifeUpdatedEvent(player, isDead);

            Emit(@event);
        }

        private void OnOxygenUpdated(Player nativePlayer, byte oxygen)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerOxygenUpdatedEvent @event = new UnturnedPlayerOxygenUpdatedEvent(player, oxygen);

            Emit(@event);
        }

        private void OnStaminaUpdated(Player nativePlayer, byte stamina)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerStaminaUpdatedEvent @event = new UnturnedPlayerStaminaUpdatedEvent(player, stamina);

            Emit(@event);
        }

        private void OnTemperatureUpdated(Player nativePlayer, EPlayerTemperature temperature)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerTemperatureUpdatedEvent @event = new UnturnedPlayerTemperatureUpdatedEvent(player, temperature);

            Emit(@event);
        }

        private void EventsOnVirusUpdated(Player nativePlayer, byte virus)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerVirusUpdatedEvent @event = new UnturnedPlayerVirusUpdatedEvent(player, virus);

            Emit(@event);
        }

        private void OnVisionUpdated(Player nativePlayer, bool viewing)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerVisionUpdatedEvent @event = new UnturnedPlayerVisionUpdatedEvent(player, viewing);

            Emit(@event);
        }

        private void EventsOnWaterUpdated(Player nativePlayer, byte water)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerWaterUpdatedEvent @event = new UnturnedPlayerWaterUpdatedEvent(player, water);

            Emit(@event);
        }

        private delegate void BleedingUpdated(Player player, bool isBleeding);
        private static event BleedingUpdated OnBleedingUpdated;

        private delegate void BrokenUpdated(Player player, bool isBroken);
        private static event BrokenUpdated OnBrokenUpdated;

        private delegate void FoodUpdated(Player player, byte food);
        private static event FoodUpdated OnFoodUpdated;

        private delegate void HealthUpdated(Player player, byte health);
        private static event HealthUpdated OnHealthUpdated;

        private delegate void VirusUpdated(Player player, byte stamina);
        private static event VirusUpdated OnVirusUpdated;

        private delegate void WaterUpdated(Player player, byte water);
        private static event WaterUpdated OnWaterUpdated;

        private struct PersistDoDamage
        {
            public bool IsBleeding;

            public byte Health;

            public PersistDoDamage(PlayerLife life)
            {
                IsBleeding = life.isBleeding;
                Health = life.health;
            }
        }

        [HarmonyPatch]
        private class Patches
        {
            [HarmonyPatch(typeof(PlayerLife), "serverSetBleeding")]
            [HarmonyPrefix]
            private static void PreServerSetBleeding(PlayerLife __instance, out bool __state)
            {
                __state = __instance.isBleeding;
            }

            [HarmonyPatch(typeof(PlayerLife), "serverSetBleeding")]
            [HarmonyPostfix]
            private static void PostServerSetBleeding(PlayerLife __instance, bool __state)
            {
                if (__instance.isBleeding != __state)
                {
                    OnBleedingUpdated?.Invoke(__instance.player, __instance.isBleeding);
                }
            }

            [HarmonyPatch(typeof(PlayerLife), "serverSetLegsBroken")]
            [HarmonyPrefix]
            private static void PreServerSetLegsBroken(PlayerLife __instance, out bool __state)
            {
                __state = __instance.isBroken;
            }

            [HarmonyPatch(typeof(PlayerLife), "serverSetLegsBroken")]
            [HarmonyPostfix]
            private static void PostServerSetLegsBroken(PlayerLife __instance, bool __state)
            {
                if (__instance.isBroken != __state)
                {
                    OnBrokenUpdated?.Invoke(__instance.player, __instance.isBroken);
                }
            }

            [HarmonyPatch(typeof(PlayerLife), "doDamage")]
            [HarmonyPrefix]
            private static void PreDoDamage(PlayerLife __instance, out PersistDoDamage __state)
            {
                __state = new PersistDoDamage(__instance);
            }

            [HarmonyPatch(typeof(PlayerLife), "doDamage")]
            [HarmonyPostfix]
            private static void PostDoDamage(PlayerLife __instance, PersistDoDamage __state)
            {
                if (__instance.isBleeding != __state.IsBleeding)
                {
                    OnBleedingUpdated?.Invoke(__instance.player, __instance.isBleeding);
                }

                if (__instance.health != __state.Health)
                {
                    OnHealthUpdated?.Invoke(__instance.player, __instance.health);
                }
            }

            [HarmonyPatch(typeof(PlayerLife), "askEat")]
            [HarmonyPrefix]
            private static void PreAskEat(PlayerLife __instance, out byte __state)
            {
                __state = __instance.food;
            }

            [HarmonyPatch(typeof(PlayerLife), "askEat")]
            [HarmonyPostfix]
            private static void PostAskEat(PlayerLife __instance, byte __state)
            {
                if (__instance.food != __state)
                {
                    OnFoodUpdated?.Invoke(__instance.player, __instance.food);
                }
            }

            [HarmonyPatch(typeof(PlayerLife), "askStarve")]
            [HarmonyPrefix]
            private static void PreAskStarve(PlayerLife __instance, out byte __state)
            {
                __state = __instance.food;
            }

            [HarmonyPatch(typeof(PlayerLife), "askStarve")]
            [HarmonyPostfix]
            private static void PostAskStarve(PlayerLife __instance, byte __state)
            {
                if (__instance.food != __state)
                {
                    OnFoodUpdated?.Invoke(__instance.player, __instance.food);
                }
            }

            [HarmonyPatch(typeof(PlayerLife), "askHeal")]
            [HarmonyPrefix]
            private static void PreAskHeal(PlayerLife __instance, out byte __state)
            {
                __state = __instance.health;
            }

            [HarmonyPatch(typeof(PlayerLife), "askHeal")]
            [HarmonyPostfix]
            private static void PostAskHeal(PlayerLife __instance, byte __state)
            {
                if (__instance.health != __state)
                {
                    OnHealthUpdated?.Invoke(__instance.player, __instance.health);
                }
            }

            [HarmonyPatch(typeof(PlayerLife), "askDisinfect")]
            [HarmonyPrefix]
            private static void PreAskDisinfect(PlayerLife __instance, out byte __state)
            {
                __state = __instance.virus;
            }

            [HarmonyPatch(typeof(PlayerLife), "askDisinfect")]
            [HarmonyPostfix]
            private static void PostAskDisinfect(PlayerLife __instance, byte __state)
            {
                if (__instance.virus != __state)
                {
                    OnVirusUpdated?.Invoke(__instance.player, __instance.virus);
                }
            }

            [HarmonyPatch(typeof(PlayerLife), "askInfect")]
            [HarmonyPrefix]
            private static void PreAskInfect(PlayerLife __instance, out byte __state)
            {
                __state = __instance.virus;
            }

            [HarmonyPatch(typeof(PlayerLife), "askInfect")]
            [HarmonyPostfix]
            private static void PostAskInfect(PlayerLife __instance, byte __state)
            {
                if (__instance.virus != __state)
                {
                    OnVirusUpdated?.Invoke(__instance.player, __instance.virus);
                }
            }

            [HarmonyPatch(typeof(PlayerLife), "askDrink")]
            [HarmonyPrefix]
            private static void PreAskDrink(PlayerLife __instance, out byte __state)
            {
                __state = __instance.water;
            }

            [HarmonyPatch(typeof(PlayerLife), "askDrink")]
            [HarmonyPostfix]
            private static void PostAskDrink(PlayerLife __instance, byte __state)
            {
                if (__instance.water != __state)
                {
                    OnWaterUpdated?.Invoke(__instance.player, __instance.water);
                }
            }

            [HarmonyPatch(typeof(PlayerLife), "askDehydrate")]
            [HarmonyPrefix]
            private static void PreAskDehydrate(PlayerLife __instance, out byte __state)
            {
                __state = __instance.water;
            }

            [HarmonyPatch(typeof(PlayerLife), "askDehydrate")]
            [HarmonyPostfix]
            private static void PostAskDehydrate(PlayerLife __instance, byte __state)
            {
                if (__instance.water != __state)
                {
                    OnWaterUpdated?.Invoke(__instance.player, __instance.water);
                }
            }
        }
    }
}
