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

        }

        public override void Unsubscribe()
        {

        }

        public override void SubscribePlayer(Player player)
        {
            player.life.onBleedingUpdated += isBleeding => OnBleedingUpdated(player, isBleeding);
            player.life.onBrokenUpdated += isBroken => OnBrokenUpdated(player, isBroken);
            player.life.onFoodUpdated += food => OnFoodUpdated(player, food);
            player.life.onHealthUpdated += health => OnHealthUpdated(player, health);
            player.life.onLifeUpdated += isDead => OnLifeUpdated(player, isDead);
            player.life.onOxygenUpdated += oxygen => OnOxygenUpdated(player, oxygen);
            player.life.onStaminaUpdated += stamina => OnStaminaUpdated(player, stamina);
            player.life.onTemperatureUpdated += temperature => OnTemperatureUpdated(player, temperature);
            player.life.onVirusUpdated += virus => OnVirusUpdated(player, virus);
            player.life.onVisionUpdated += viewing => OnVisionUpdated(player, viewing);
        }

        public override void UnsubscribePlayer(Player player)
        {
            player.life.onBleedingUpdated -= isBleeding => OnBleedingUpdated(player, isBleeding);
            player.life.onBrokenUpdated -= isBroken => OnBrokenUpdated(player, isBroken);
            player.life.onFoodUpdated -= food => OnFoodUpdated(player, food);
            player.life.onHealthUpdated -= health => OnHealthUpdated(player, health);
            player.life.onLifeUpdated -= isDead => OnLifeUpdated(player, isDead);
            player.life.onOxygenUpdated -= oxygen => OnOxygenUpdated(player, oxygen);
            player.life.onStaminaUpdated -= stamina => OnStaminaUpdated(player, stamina);
            player.life.onTemperatureUpdated -= temperature => OnTemperatureUpdated(player, temperature);
            player.life.onVirusUpdated -= virus => OnVirusUpdated(player, virus);
            player.life.onVisionUpdated -= viewing => OnVisionUpdated(player, viewing);
        }

        private void OnBleedingUpdated(Player nativePlayer, bool isBleeding)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerBleedingUpdateEvent @event = new UnturnedPlayerBleedingUpdateEvent(player, isBleeding);

            Emit(@event);
        }

        private void OnBrokenUpdated(Player nativePlayer, bool isBroken)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerBrokenUpdateEvent @event = new UnturnedPlayerBrokenUpdateEvent(player, isBroken);

            Emit(@event);
        }

        private void OnFoodUpdated(Player nativePlayer, byte food)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerFoodUpdateEvent @event = new UnturnedPlayerFoodUpdateEvent(player, food);

            Emit(@event);
        }

        private void OnHealthUpdated(Player nativePlayer, byte health)
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

        private void OnVirusUpdated(Player nativePlayer, byte virus)
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

        private delegate void WaterUpdateHandler(Player player, byte water);
        private static event WaterUpdateHandler OnWaterUpdate;

        private delegate void FoodUpdateHandler(Player player, byte food);
        private static event FoodUpdateHandler OnFoodUpdate;

        [HarmonyPatch]
        private class Patches
        {
            [HarmonyPatch(typeof(PlayerLife), "askDrink")]
            [HarmonyPrefix]
            private static void PreAskDrink(PlayerLife __instance, out byte __state)
            {
                System.Console.WriteLine("PreAsk" + __instance.water);
                __state = __instance.water;
            }

            [HarmonyPatch(typeof(PlayerLife), "askDrink")]
            [HarmonyPostfix]
            private static void PostAskDrink(PlayerLife __instance, byte __state)
            {
                System.Console.WriteLine("PostAsk " + __instance.water);
                if (__instance.water != __state)
                {
                    OnWaterUpdate?.Invoke(__instance.player, __instance.water);
                }
            }
        }
    }
}
