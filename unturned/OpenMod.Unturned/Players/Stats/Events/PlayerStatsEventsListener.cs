extern alias JetBrainsAnnotations;
using HarmonyLib;
using JetBrainsAnnotations::JetBrains.Annotations;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.Unturned.Events;
using SDG.Unturned;

namespace OpenMod.Unturned.Players.Stats.Events
{
    [UsedImplicitly]
    internal class PlayerStatsEventsListener : UnturnedPlayerEventsListener
    {
        public PlayerStatsEventsListener(IOpenModHost openModHost,
            IEventBus eventBus,
            IUserManager userManager) : base(openModHost, eventBus, userManager)
        {
        }

        public override void Subscribe()
        {
            Player.onPlayerStatIncremented += OnPlayerStatIncremented;
            OnBleedingUpdated += Events_OnBleedingUpdated;
            OnBrokenUpdated += Events_OnBrokenUpdated;
            OnFoodUpdated += Events_OnFoodUpdated;
            OnHealthUpdated += Events_OnHealthUpdated;
            OnVirusUpdated += Events_OnVirusUpdated;
            OnWaterUpdated += Events_OnWaterUpdated;
        }

        public override void Unsubscribe()
        {
            Player.onPlayerStatIncremented -= OnPlayerStatIncremented;
            OnBleedingUpdated -= Events_OnBleedingUpdated;
            OnBrokenUpdated -= Events_OnBrokenUpdated;
            OnFoodUpdated -= Events_OnFoodUpdated;
            OnHealthUpdated -= Events_OnHealthUpdated;
            OnVirusUpdated -= Events_OnVirusUpdated;
            OnWaterUpdated -= Events_OnWaterUpdated;
        }

        public override void SubscribePlayer(Player player)
        {
            player.life.onLifeUpdated += isDead => OnLifeUpdated(player, isDead);
            player.life.onOxygenUpdated += oxygen => OnOxygenUpdated(player, oxygen);
            player.life.onStaminaUpdated += stamina => OnStaminaUpdated(player, stamina);
            player.life.onTemperatureUpdated += temperature => OnTemperatureUpdated(player, temperature);
            player.life.onVirusUpdated += virus => Events_OnVirusUpdated(player, virus);
            player.life.onVisionUpdated += viewing => OnVisionUpdated(player, viewing);
        }

        public override void UnsubscribePlayer(Player player)
        {
            player.life.onLifeUpdated -= isDead => OnLifeUpdated(player, isDead);
            player.life.onOxygenUpdated -= oxygen => OnOxygenUpdated(player, oxygen);
            player.life.onStaminaUpdated -= stamina => OnStaminaUpdated(player, stamina);
            player.life.onTemperatureUpdated -= temperature => OnTemperatureUpdated(player, temperature);
            player.life.onVirusUpdated -= virus => Events_OnVirusUpdated(player, virus);
            player.life.onVisionUpdated -= viewing => OnVisionUpdated(player, viewing);
        }

        private void Events_OnBleedingUpdated(Player nativePlayer, bool isBleeding)
        {
            var player = GetUnturnedPlayer(nativePlayer)!;
            var @event = new UnturnedPlayerBleedingUpdatedEvent(player, isBleeding);

            Emit(@event);
        }

        private void Events_OnBrokenUpdated(Player nativePlayer, bool isBroken)
        {
            var player = GetUnturnedPlayer(nativePlayer)!;
            var @event = new UnturnedPlayerBrokenUpdatedEvent(player, isBroken);

            Emit(@event);
        }

        private void Events_OnFoodUpdated(Player nativePlayer, byte food)
        {
            var player = GetUnturnedPlayer(nativePlayer)!;
            var @event = new UnturnedPlayerFoodUpdatedEvent(player, food);

            Emit(@event);
        }

        private void Events_OnHealthUpdated(Player nativePlayer, byte health)
        {
            var player = GetUnturnedPlayer(nativePlayer)!;
            var @event = new UnturnedPlayerHealthUpdatedEvent(player, health);

            Emit(@event);
        }

        private void OnPlayerStatIncremented(Player nativePlayer, EPlayerStat stat)
        {
            var player = GetUnturnedPlayer(nativePlayer)!;
            var @event = new UnturnedPlayerStatIncrementedEvent(player, stat);

            Emit(@event);
        }

        private void OnLifeUpdated(Player nativePlayer, bool isDead)
        {
            var player = GetUnturnedPlayer(nativePlayer)!;
            var @event = new UnturnedPlayerLifeUpdatedEvent(player, isDead);

            Emit(@event);
        }

        private void OnOxygenUpdated(Player nativePlayer, byte oxygen)
        {
            var player = GetUnturnedPlayer(nativePlayer)!;
            var @event = new UnturnedPlayerOxygenUpdatedEvent(player, oxygen);

            Emit(@event);
        }

        private void OnStaminaUpdated(Player nativePlayer, byte stamina)
        {
            var player = GetUnturnedPlayer(nativePlayer)!;
            var @event = new UnturnedPlayerStaminaUpdatedEvent(player, stamina);

            Emit(@event);
        }

        private void OnTemperatureUpdated(Player nativePlayer, EPlayerTemperature temperature)
        {
            var player = GetUnturnedPlayer(nativePlayer)!;
            var @event = new UnturnedPlayerTemperatureUpdatedEvent(player, temperature);

            Emit(@event);
        }

        private void Events_OnVirusUpdated(Player nativePlayer, byte virus)
        {
            var player = GetUnturnedPlayer(nativePlayer)!;
            var @event = new UnturnedPlayerVirusUpdatedEvent(player, virus);

            Emit(@event);
        }

        private void OnVisionUpdated(Player nativePlayer, bool viewing)
        {
            var player = GetUnturnedPlayer(nativePlayer)!;
            var @event = new UnturnedPlayerVisionUpdatedEvent(player, viewing);

            Emit(@event);
        }

        private void Events_OnWaterUpdated(Player nativePlayer, byte water)
        {
            var player = GetUnturnedPlayer(nativePlayer)!;
            var @event = new UnturnedPlayerWaterUpdatedEvent(player, water);

            Emit(@event);
        }

        private delegate void BleedingUpdated(Player player, bool isBleeding);
        private static event BleedingUpdated? OnBleedingUpdated;

        private delegate void BrokenUpdated(Player player, bool isBroken);
        private static event BrokenUpdated? OnBrokenUpdated;

        private delegate void FoodUpdated(Player player, byte food);
        private static event FoodUpdated? OnFoodUpdated;

        private delegate void HealthUpdated(Player player, byte health);
        private static event HealthUpdated? OnHealthUpdated;

        private delegate void VirusUpdated(Player player, byte stamina);
        private static event VirusUpdated? OnVirusUpdated;

        private delegate void WaterUpdated(Player player, byte water);
        private static event WaterUpdated? OnWaterUpdated;

        internal readonly struct PersistDoDamage
        {
            public readonly bool IsBleeding;

            public readonly byte Health;

            public PersistDoDamage(PlayerLife life)
            {
                IsBleeding = life.isBleeding;
                Health = life.health;
            }
        }

        [UsedImplicitly]
        [HarmonyPatch]
        internal static class Patches
        {
            [UsedImplicitly]
            [HarmonyPatch(typeof(PlayerLife), "serverSetBleeding")]
            [HarmonyPrefix]
            public static void PreServerSetBleeding(PlayerLife __instance, out bool __state)
            {
                __state = __instance.isBleeding;
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(PlayerLife), "serverSetBleeding")]
            [HarmonyPostfix]
            public static void PostServerSetBleeding(PlayerLife __instance, bool __state)
            {
                if (__instance.isBleeding != __state)
                {
                    OnBleedingUpdated?.Invoke(__instance.player, __instance.isBleeding);
                }
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(PlayerLife), "serverSetLegsBroken")]
            [HarmonyPrefix]
            public static void PreServerSetLegsBroken(PlayerLife __instance, out bool __state)
            {
                __state = __instance.isBroken;
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(PlayerLife), "serverSetLegsBroken")]
            [HarmonyPostfix]
            public static void PostServerSetLegsBroken(PlayerLife __instance, bool __state)
            {
                if (__instance.isBroken != __state)
                {
                    OnBrokenUpdated?.Invoke(__instance.player, __instance.isBroken);
                }
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(PlayerLife), "doDamage")]
            [HarmonyPrefix]
            public static void PreDoDamage(PlayerLife __instance, out PersistDoDamage __state)
            {
                __state = new PersistDoDamage(__instance);
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(PlayerLife), "doDamage")]
            [HarmonyPostfix]
            public static void PostDoDamage(PlayerLife __instance, PersistDoDamage __state)
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

            [UsedImplicitly]
            [HarmonyPatch(typeof(PlayerLife), "askEat")]
            [HarmonyPrefix]
            public static void PreAskEat(PlayerLife __instance, out byte __state)
            {
                __state = __instance.food;
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(PlayerLife), "askEat")]
            [HarmonyPostfix]
            public static void PostAskEat(PlayerLife __instance, byte __state)
            {
                if (__instance.food != __state)
                {
                    OnFoodUpdated?.Invoke(__instance.player, __instance.food);
                }
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(PlayerLife), "askStarve")]
            [HarmonyPrefix]
            public static void PreAskStarve(PlayerLife __instance, out byte __state)
            {
                __state = __instance.food;
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(PlayerLife), "askStarve")]
            [HarmonyPostfix]
            public static void PostAskStarve(PlayerLife __instance, byte __state)
            {
                if (__instance.food != __state)
                {
                    OnFoodUpdated?.Invoke(__instance.player, __instance.food);
                }
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(PlayerLife), "askHeal")]
            [HarmonyPrefix]
            public static void PreAskHeal(PlayerLife __instance, out byte __state)
            {
                __state = __instance.health;
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(PlayerLife), "askHeal")]
            [HarmonyPostfix]
            public static void PostAskHeal(PlayerLife __instance, byte __state)
            {
                if (__instance.health != __state)
                {
                    OnHealthUpdated?.Invoke(__instance.player, __instance.health);
                }
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(PlayerLife), "askDisinfect")]
            [HarmonyPrefix]
            public static void PreAskDisinfect(PlayerLife __instance, out byte __state)
            {
                __state = __instance.virus;
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(PlayerLife), "askDisinfect")]
            [HarmonyPostfix]
            public static void PostAskDisinfect(PlayerLife __instance, byte __state)
            {
                if (__instance.virus != __state)
                {
                    OnVirusUpdated?.Invoke(__instance.player, __instance.virus);
                }
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(PlayerLife), "askInfect")]
            [HarmonyPrefix]
            public static void PreAskInfect(PlayerLife __instance, out byte __state)
            {
                __state = __instance.virus;
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(PlayerLife), "askInfect")]
            [HarmonyPostfix]
            public static void PostAskInfect(PlayerLife __instance, byte __state)
            {
                if (__instance.virus != __state)
                {
                    OnVirusUpdated?.Invoke(__instance.player, __instance.virus);
                }
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(PlayerLife), "askDrink")]
            [HarmonyPrefix]
            public static void PreAskDrink(PlayerLife __instance, out byte __state)
            {
                __state = __instance.water;
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(PlayerLife), "askDrink")]
            [HarmonyPostfix]
            public static void PostAskDrink(PlayerLife __instance, byte __state)
            {
                if (__instance.water != __state)
                {
                    OnWaterUpdated?.Invoke(__instance.player, __instance.water);
                }
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(PlayerLife), "askDehydrate")]
            [HarmonyPrefix]
            public static void PreAskDehydrate(PlayerLife __instance, out byte __state)
            {
                __state = __instance.water;
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(PlayerLife), "askDehydrate")]
            [HarmonyPostfix]
            public static void PostAskDehydrate(PlayerLife __instance, byte __state)
            {
                if (__instance.water != __state)
                {
                    OnWaterUpdated?.Invoke(__instance.player, __instance.water);
                }
            }
        }
    }
}
