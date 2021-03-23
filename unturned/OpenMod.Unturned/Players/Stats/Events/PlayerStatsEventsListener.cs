extern alias JetBrainsAnnotations;
using System;
using JetBrainsAnnotations::JetBrains.Annotations;
using OpenMod.Unturned.Events;
using SDG.Unturned;

namespace OpenMod.Unturned.Players.Stats.Events
{
    [UsedImplicitly]
    internal class PlayerStatsEventsListener : UnturnedPlayerEventsListener
    {
        public PlayerStatsEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override void Subscribe()
        {
            Player.onPlayerStatIncremented += OnPlayerStatIncremented;
            PlayerLife.OnTellBleeding_Global += OnTellBleeding_Global;
            PlayerLife.OnTellBroken_Global += OnTellBroken_Global;
            PlayerLife.OnTellFood_Global += OnTellFood_Global;
            PlayerLife.OnTellHealth_Global += OnTellHealth_Global;
            PlayerLife.OnTellVirus_Global += OnTellVirus_Global;
            PlayerLife.OnTellWater_Global += OnTellWater_Global;
        }

        public override void Unsubscribe()
        {
            Player.onPlayerStatIncremented -= OnPlayerStatIncremented;
            PlayerLife.OnTellBleeding_Global -= OnTellBleeding_Global;
            PlayerLife.OnTellBroken_Global -= OnTellBroken_Global;
            PlayerLife.OnTellFood_Global -= OnTellFood_Global;
            PlayerLife.OnTellHealth_Global -= OnTellHealth_Global;
            PlayerLife.OnTellVirus_Global -= OnTellVirus_Global;
            PlayerLife.OnTellWater_Global -= OnTellWater_Global;
        }

        public override void SubscribePlayer(Player player)
        {
            player.life.onLifeUpdated += isDead => OnLifeUpdated(player, isDead);
            player.life.onOxygenUpdated += oxygen => OnOxygenUpdated(player, oxygen);
            player.life.onStaminaUpdated += stamina => OnStaminaUpdated(player, stamina);
            player.life.onTemperatureUpdated += temperature => OnTemperatureUpdated(player, temperature);
            player.life.onVisionUpdated += viewing => OnVisionUpdated(player, viewing);
        }

        public override void UnsubscribePlayer(Player player)
        {
            player.life.onLifeUpdated -= isDead => OnLifeUpdated(player, isDead);
            player.life.onOxygenUpdated -= oxygen => OnOxygenUpdated(player, oxygen);
            player.life.onStaminaUpdated -= stamina => OnStaminaUpdated(player, stamina);
            player.life.onTemperatureUpdated -= temperature => OnTemperatureUpdated(player, temperature);
            player.life.onVisionUpdated -= viewing => OnVisionUpdated(player, viewing);
        }

        private void OnTellWater_Global(PlayerLife life)
        {
            var player = GetUnturnedPlayer(life.player)!;
            var @event = new UnturnedPlayerWaterUpdatedEvent(player, life.water);

            Emit(@event);
        }

        private void OnTellVirus_Global(PlayerLife life)
        {
            var player = GetUnturnedPlayer(life.player)!;
            var @event = new UnturnedPlayerVirusUpdatedEvent(player, life.virus);

            Emit(@event);
        }

        private void OnTellHealth_Global(PlayerLife life)
        {
            var player = GetUnturnedPlayer(life.player)!;
            var @event = new UnturnedPlayerHealthUpdatedEvent(player, life.health);

            Emit(@event);
        }

        private void OnTellFood_Global(PlayerLife life)
        {
            var player = GetUnturnedPlayer(life.player)!;
            var @event = new UnturnedPlayerFoodUpdatedEvent(player, life.food);

            Emit(@event);
        }

        private void OnTellBroken_Global(PlayerLife life)
        {
            var player = GetUnturnedPlayer(life.player)!;
            var @event = new UnturnedPlayerBrokenUpdatedEvent(player, life.isBroken);

            Emit(@event);
        }

        private void OnTellBleeding_Global(PlayerLife life)
        {
            var player = GetUnturnedPlayer(life.player)!;
            var @event = new UnturnedPlayerBleedingUpdatedEvent(player, life.isBleeding);

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

        private void OnVisionUpdated(Player nativePlayer, bool viewing)
        {
            var player = GetUnturnedPlayer(nativePlayer)!;
            var @event = new UnturnedPlayerVisionUpdatedEvent(player, viewing);

            Emit(@event);
        }
    }
}
