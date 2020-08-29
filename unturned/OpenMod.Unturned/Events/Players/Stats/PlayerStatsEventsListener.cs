using HarmonyLib;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.Unturned.Entities;
using SDG.Unturned;

namespace OpenMod.Unturned.Events.Players.Stats
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
            OnBleedingUpdate += Events_OnBleedingUpdate;
            OnBrokenUpdate += Events_OnBrokenUpdate;
            OnFoodUpdate += Events_OnFoodUpdate;
            OnHealthUpdate += Events_OnHealthUpdate;
            OnVirusUpdate += Events_OnVirusUpdate;
            OnWaterUpdate += Events_OnWaterUpdate;
        }

        public override void Unsubscribe()
        {
            OnBleedingUpdate -= Events_OnBleedingUpdate;
            OnBrokenUpdate -= Events_OnBrokenUpdate;
            OnFoodUpdate -= Events_OnFoodUpdate;
            OnHealthUpdate -= Events_OnHealthUpdate;
            OnVirusUpdate -= Events_OnVirusUpdate;
            OnWaterUpdate -= Events_OnWaterUpdate;
        }

        public override void SubscribePlayer(Player player)
        {
            player.life.onLifeUpdated += isDead => OnLifeUpdated(player, isDead);

            player.life.onOxygenUpdated += oxygen => OnOxygenUpdated(player, oxygen);
            player.life.onStaminaUpdated += stamina => OnStaminaUpdated(player, stamina);
            player.life.onTemperatureUpdated += temperature => OnTemperatureUpdated(player, temperature);
            player.life.onVirusUpdated += virus => Events_OnVirusUpdate(player, virus);
            player.life.onVisionUpdated += viewing => OnVisionUpdated(player, viewing);
        }

        public override void UnsubscribePlayer(Player player)
        {
            player.life.onLifeUpdated -= isDead => OnLifeUpdated(player, isDead);

            player.life.onOxygenUpdated -= oxygen => OnOxygenUpdated(player, oxygen);
            player.life.onStaminaUpdated -= stamina => OnStaminaUpdated(player, stamina);
            player.life.onTemperatureUpdated -= temperature => OnTemperatureUpdated(player, temperature);
            player.life.onVirusUpdated -= virus => Events_OnVirusUpdate(player, virus);
            player.life.onVisionUpdated -= viewing => OnVisionUpdated(player, viewing);
        }

        private void Events_OnBleedingUpdate(Player nativePlayer, bool isBleeding)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerBleedingUpdateEvent @event = new UnturnedPlayerBleedingUpdateEvent(player, isBleeding);

            Emit(@event);
        }

        private void Events_OnBrokenUpdate(Player nativePlayer, bool isBroken)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerBrokenUpdateEvent @event = new UnturnedPlayerBrokenUpdateEvent(player, isBroken);

            Emit(@event);
        }

        private void Events_OnFoodUpdate(Player nativePlayer, byte food)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerFoodUpdateEvent @event = new UnturnedPlayerFoodUpdateEvent(player, food);

            Emit(@event);
        }

        private void Events_OnHealthUpdate(Player nativePlayer, byte health)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerHealthUpdateEvent @event = new UnturnedPlayerHealthUpdateEvent(player, health);

            Emit(@event);
        }

        private void OnLifeUpdated(Player nativePlayer, bool isDead)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerLifeUpdateEvent @event = new UnturnedPlayerLifeUpdateEvent(player, isDead);

            Emit(@event);
        }

        private void OnOxygenUpdated(Player nativePlayer, byte oxygen)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerOxygenUpdateEvent @event = new UnturnedPlayerOxygenUpdateEvent(player, oxygen);

            Emit(@event);
        }

        private void OnStaminaUpdated(Player nativePlayer, byte stamina)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerStaminaUpdateEvent @event = new UnturnedPlayerStaminaUpdateEvent(player, stamina);

            Emit(@event);
        }

        private void OnTemperatureUpdated(Player nativePlayer, EPlayerTemperature temperature)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerTemperatureUpdateEvent @event = new UnturnedPlayerTemperatureUpdateEvent(player, temperature);

            Emit(@event);
        }

        private void Events_OnVirusUpdate(Player nativePlayer, byte virus)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerVirusUpdateEvent @event = new UnturnedPlayerVirusUpdateEvent(player, virus);

            Emit(@event);
        }

        private void OnVisionUpdated(Player nativePlayer, bool viewing)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerVisionUpdateEvent @event = new UnturnedPlayerVisionUpdateEvent(player, viewing);

            Emit(@event);
        }

        private void Events_OnWaterUpdate(Player nativePlayer, byte water)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerWaterUpdateEvent @event = new UnturnedPlayerWaterUpdateEvent(player, water);

            Emit(@event);
        }

        private delegate void BleedingUpdate(Player player, bool isBleeding);
        private static event BleedingUpdate OnBleedingUpdate;

        private delegate void BrokenUpdate(Player player, bool isBroken);
        private static event BrokenUpdate OnBrokenUpdate;

        private delegate void FoodUpdate(Player player, byte food);
        private static event FoodUpdate OnFoodUpdate;

        private delegate void HealthUpdate(Player player, byte health);
        private static event HealthUpdate OnHealthUpdate;

        private delegate void VirusUpdate(Player player, byte stamina);
        private static event VirusUpdate OnVirusUpdate;

        private delegate void WaterUpdate(Player player, byte water);
        private static event WaterUpdate OnWaterUpdate;

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
                    OnBleedingUpdate?.Invoke(__instance.player, __instance.isBleeding);
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
                    OnBrokenUpdate?.Invoke(__instance.player, __instance.isBroken);
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
                    OnBleedingUpdate?.Invoke(__instance.player, __instance.isBleeding);
                }

                if (__instance.health != __state.Health)
                {
                    OnHealthUpdate?.Invoke(__instance.player, __instance.health);
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
                    OnFoodUpdate?.Invoke(__instance.player, __instance.food);
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
                    OnFoodUpdate?.Invoke(__instance.player, __instance.food);
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
                    OnHealthUpdate?.Invoke(__instance.player, __instance.health);
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
                    OnVirusUpdate?.Invoke(__instance.player, __instance.virus);
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
                    OnVirusUpdate?.Invoke(__instance.player, __instance.virus);
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
                    OnWaterUpdate?.Invoke(__instance.player, __instance.water);
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
                    OnWaterUpdate?.Invoke(__instance.player, __instance.water);
                }
            }
        }
    }
}
