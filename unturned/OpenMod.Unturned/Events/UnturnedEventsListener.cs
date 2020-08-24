using HarmonyLib;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.Core.Helpers;
using OpenMod.Core.Users;
using OpenMod.Unturned.Entities;
using OpenMod.Unturned.Events.Bans;
using OpenMod.Unturned.Events.Chat;
using OpenMod.Unturned.Events.Environment;
using OpenMod.Unturned.Events.Players;
using OpenMod.Unturned.Events.Players.Crafting;
using OpenMod.Unturned.Events.Players.Equipment;
using OpenMod.Unturned.Events.Players.Inventory;
using OpenMod.Unturned.Events.Players.Stats;
using OpenMod.Unturned.Events.Vehicles;
using OpenMod.Unturned.Events.Zombies;
using SDG.Unturned;
using Steamworks;
using System.Linq;
using UnityEngine;

namespace OpenMod.Unturned.Events
{
    internal class UnturnedEventsListener
    {
        private readonly IOpenModHost m_OpenModHost;
        private readonly IEventBus m_EventBus;
        private readonly IUserManager m_UserManager;

        public UnturnedEventsListener(
            IOpenModHost openModHost,
            IEventBus eventBus,
            IUserManager userManager)
        {
            m_OpenModHost = openModHost;
            m_EventBus = eventBus;
            m_UserManager = userManager;
        }

        public void Subscribe()
        {
            Provider.clients.Do(SubscribePlayer);
            Provider.onEnemyConnected += SubscribePlayer;
            Provider.onEnemyDisconnected += UnsubscribePlayer;

            Provider.onBanPlayerRequested += OnBanPlayerRequested;
            Provider.onCheckBanStatusWithHWID += OnCheckBanStatus;
            Provider.onUnbanPlayerRequested += OnUnbanPlayerRequested;

            ChatManager.onChatted += OnChatted;
            ChatManager.onServerSendingMessage += OnServerSendingMessage;

            LightingManager.onDayNightUpdated += OnDayNightUpdated;
            LightingManager.onMoonUpdated += OnMoonUpdated;
            LightingManager.onRainUpdated += OnRainUpdated;
            LightingManager.onSnowUpdated += OnSnowUpdated;

            Provider.onEnemyConnected += OnPlayerConnected;
            Provider.onEnemyDisconnected += OnPlayerDisconnected;

            OnDoDamage += Events_OnDoDamage;
            OnTeleport += Events_OnTeleport;
            OnDead += Events_OnDead;

            UseableConsumeable.onPerformingAid += OnPerformingAid;
            PlayerCrafting.onCraftBlueprintRequested += OnCraftBlueprintRequested;
            PlayerLife.onPlayerDied += OnPlayerDeath;

            VehicleManager.onEnterVehicleRequested += OnEnterVehicleRequested;
            VehicleManager.onExitVehicleRequested += OnExitVehicleRequested;
            VehicleManager.onSwapSeatRequested += OnSwapSeatRequested;
            VehicleManager.onDamageTireRequested += OnDamageTireRequested;
            VehicleManager.onDamageVehicleRequested += OnDamageVehicleRequested;
            VehicleManager.onRepairVehicleRequested += OnRepairVehicleRequested;
            VehicleManager.onSiphonVehicleRequested += OnSiphonVehicleRequested;
            VehicleManager.onVehicleCarjacked += OnVehicleCarjacked;
            VehicleManager.onVehicleLockpicked += OnVehicleLockpicked;
            OnVehicleExplode += Events_OnVehicleExplode;
            OnVehicleSpawn += Events_OnVehicleSpawn;

            OnZombieSpawn += Events_OnZombieSpawn;
        }

        private void Events_OnZombieSpawn(Zombie zombie)
        {
            UnturnedZombieSpawnEvent @event = new UnturnedZombieSpawnEvent(zombie);

            Emit(@event);
        }

        public void Unsubscribe()
        {
            // ReSharper disable DelegateSubtraction
            Provider.clients.Do(UnsubscribePlayer);
            Provider.onEnemyConnected -= SubscribePlayer;
            Provider.onEnemyDisconnected -= UnsubscribePlayer;

            Provider.onBanPlayerRequested -= OnBanPlayerRequested;
            Provider.onCheckBanStatusWithHWID -= OnCheckBanStatus;
            Provider.onUnbanPlayerRequested -= OnUnbanPlayerRequested;

            ChatManager.onChatted -= OnChatted;
            ChatManager.onServerSendingMessage -= OnServerSendingMessage;

            LightingManager.onDayNightUpdated -= OnDayNightUpdated;
            LightingManager.onMoonUpdated -= OnMoonUpdated;
            LightingManager.onRainUpdated -= OnRainUpdated;
            LightingManager.onSnowUpdated -= OnSnowUpdated;

            Provider.onEnemyConnected -= OnPlayerConnected;
            Provider.onEnemyDisconnected -= OnPlayerDisconnected;

            OnDoDamage -= Events_OnDoDamage;
            OnTeleport -= Events_OnTeleport;
            OnDead -= Events_OnDead;

            UseableConsumeable.onPerformingAid -= OnPerformingAid;
            PlayerCrafting.onCraftBlueprintRequested -= OnCraftBlueprintRequested;
            PlayerLife.onPlayerDied -= OnPlayerDeath;

            VehicleManager.onEnterVehicleRequested -= OnEnterVehicleRequested;
            VehicleManager.onExitVehicleRequested -= OnExitVehicleRequested;
            VehicleManager.onSwapSeatRequested -= OnSwapSeatRequested;
            VehicleManager.onDamageTireRequested -= OnDamageTireRequested;
            VehicleManager.onDamageVehicleRequested -= OnDamageVehicleRequested;
            VehicleManager.onRepairVehicleRequested -= OnRepairVehicleRequested;
            VehicleManager.onSiphonVehicleRequested -= OnSiphonVehicleRequested;
            VehicleManager.onVehicleCarjacked -= OnVehicleCarjacked;
            VehicleManager.onVehicleLockpicked -= OnVehicleLockpicked;
            OnVehicleExplode -= Events_OnVehicleExplode;
            OnVehicleSpawn -= Events_OnVehicleSpawn;

            OnZombieSpawn -= Events_OnZombieSpawn;
            // ReSharper restore DelegateSubtraction
        }

        public void SubscribePlayer(SteamPlayer steamPlayer)
        {
            Player player = steamPlayer.player;

            player.equipment.onEquipRequested += OnEquipRequested;
            player.equipment.onDequipRequested += OnDequipRequested;

            ItemManager.onTakeItemRequested += OnTakeItemRequested;
            player.inventory.onDropItemRequested += OnDropItemRequested;
            player.inventory.onInventoryResized +=
                (page, width, height) => OnInventoryResized(player, page, width, height);
            player.inventory.onInventoryStored += () => OnInventoryStored(player);
            player.inventory.onInventoryStateUpdated += () => OnInventoryStateUpdated(player);
            player.inventory.onInventoryAdded += (page, index, jar) => OnInventoryAdded(player, page, index, jar);
            player.inventory.onInventoryRemoved += (page, index, jar) => OnInventoryRemoved(player, page, index, jar);
            player.inventory.onInventoryUpdated += (page, index, jar) => OnInventoryUpdated(player, page, index, jar);

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
            player.life.onWaterUpdated += water => OnWaterUpdated(player, water);

            player.stance.onStanceUpdated += () => OnStanceUpdated(player);
        }

        public void UnsubscribePlayer(SteamPlayer steamPlayer)
        {
            Player player = steamPlayer.player;

            // ReSharper disable DelegateSubtraction
            player.equipment.onEquipRequested -= OnEquipRequested;
            player.equipment.onDequipRequested -= OnDequipRequested;

            ItemManager.onTakeItemRequested -= OnTakeItemRequested;
            player.inventory.onDropItemRequested -= OnDropItemRequested;
            player.inventory.onInventoryResized -=
                (page, width, height) => OnInventoryResized(player, page, width, height);
            player.inventory.onInventoryStored -= () => OnInventoryStored(player);
            player.inventory.onInventoryStateUpdated -= () => OnInventoryStateUpdated(player);
            player.inventory.onInventoryAdded -= (page, index, jar) => OnInventoryAdded(player, page, index, jar);
            player.inventory.onInventoryRemoved -= (page, index, jar) => OnInventoryRemoved(player, page, index, jar);
            player.inventory.onInventoryUpdated -= (page, index, jar) => OnInventoryUpdated(player, page, index, jar);

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
            player.life.onWaterUpdated -= water => OnWaterUpdated(player, water);

            player.stance.onStanceUpdated -= () => OnStanceUpdated(player);
            // ReSharper restore DelegateSubtraction
        }

        private UnturnedPlayer GetUnturnedPlayer(Player player)
        {
            return AsyncHelper.RunSync(() => m_UserManager.FindUserAsync(KnownActorTypes.Player,
                player.channel.owner.playerID.steamID.ToString(), UserSearchMode.FindById)) as UnturnedPlayer;
        }

        private void Emit(IEvent @event)
        {
            AsyncHelper.RunSync(() => m_EventBus.EmitAsync(m_OpenModHost, this, @event));
        }

        #region Static Events

        private delegate void DoDamageHandler(Player player, ref byte amount,
            ref EDeathCause cause, ref ELimb limb,
            ref CSteamID killer, ref bool trackKill,
            ref Vector3 ragdoll, ref ERagdollEffect ragdollEffect,
            ref bool canCauseBleeding, out bool cancel);
        private static event DoDamageHandler OnDoDamage;

        private delegate void TeleportHandler(Player player,
            ref Vector3 position, ref float yaw, out bool cancel);
        private static event TeleportHandler OnTeleport;

        private delegate void DeadHandler(Player player,
            Vector3 ragdoll, byte ragdollEffect);
        private static event DeadHandler OnDead;

        private delegate void VehicleExplode(InteractableVehicle vehicle, out bool cancel);
        private static event VehicleExplode OnVehicleExplode;

        private delegate void VehicleSpawn(InteractableVehicle vehicle);
        private static event VehicleSpawn OnVehicleSpawn;

        private delegate void ZombieSpawn(Zombie zombie);
        private static event ZombieSpawn OnZombieSpawn;

        #endregion

        #region Hooked Events

        #region Bans
        private void OnBanPlayerRequested(CSteamID instigator, CSteamID playerToBan, uint ipToBan, ref string reason, ref uint duration, ref bool shouldVanillaBan)
        {
            UnturnedBanPlayerEvent @event = new UnturnedBanPlayerEvent(instigator, playerToBan, ipToBan, reason, duration, shouldVanillaBan);

            Emit(@event);

            reason = @event.Reason;
            duration = @event.Duration;
            shouldVanillaBan = @event.ShouldVanillaBan;
        }

        private void OnCheckBanStatus(SteamPlayerID playerId, uint remoteIP, ref bool isBanned, ref string banReason, ref uint banRemainingDuration)
        {
            UnturnedCheckBanStatusEvent @event = new UnturnedCheckBanStatusEvent(playerId, remoteIP, isBanned, banReason, banRemainingDuration);

            Emit(@event);

            isBanned = @event.IsBanned;
            banReason = @event.Reason;
            banRemainingDuration = @event.RemainingDuration;
        }

        private void OnUnbanPlayerRequested(CSteamID instigator, CSteamID playerToUnban, ref bool shouldVanillaUnban)
        {
            UnturnedUnbanPlayerEvent @event = new UnturnedUnbanPlayerEvent(instigator, playerToUnban, shouldVanillaUnban);

            Emit(@event);

            shouldVanillaUnban = @event.ShouldVanillaUnban;
        }

        #endregion

        #region Chat

        private void OnChatted(SteamPlayer steamPlayer, EChatMode mode, ref Color color, ref bool isRich, string text, ref bool isVisible)
        {
            UnturnedPlayer player = GetUnturnedPlayer(steamPlayer.player);

            UnturnedPlayerChatEvent @event = new UnturnedPlayerChatEvent(player, mode, color, isRich, text);

            Emit(@event);

            color = @event.Color;
            isRich = @event.IsRich;
            isVisible = !@event.IsCancelled;
        }

        private void OnServerSendingMessage(ref string text, ref Color color, SteamPlayer nativeFromPlayer, SteamPlayer nativeToPlayer, EChatMode mode, ref string iconURL, ref bool useRichTextFormatting)
        {
            UnturnedPlayer fromPlayer = GetUnturnedPlayer(nativeFromPlayer.player);
            UnturnedPlayer toPlayer = GetUnturnedPlayer(nativeToPlayer.player);

            UnturnedServerSendMessageEvent @event = new UnturnedServerSendMessageEvent(fromPlayer, toPlayer, text, color, mode, iconURL, useRichTextFormatting);

            Emit(@event);

            text = @event.Text;
            color = @event.Color;
            iconURL = @event.IconUrl;
            useRichTextFormatting = @event.IsRich;
        }

        #endregion

        #region Environment

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

        #endregion

        #region Players

        [HarmonyPatch]
        private class PlayerPatches
        {
            [HarmonyPatch(typeof(PlayerLife), "doDamage")]
            [HarmonyPrefix]
            private static bool DoDamage(PlayerLife __instance, ref byte amount,
                ref EDeathCause newCause, ref ELimb newLimb,
                ref CSteamID newKiller, ref bool trackKill,
                ref Vector3 newRagdoll, ref ERagdollEffect newRagdollEffect,
                ref bool canCauseBleeding)
            {
                bool cancel = false;

                OnDoDamage?.Invoke(__instance.player, ref amount,
                    ref newCause, ref newLimb,
                    ref newKiller, ref trackKill,
                    ref newRagdoll, ref newRagdollEffect,
                    ref canCauseBleeding, out cancel);

                return !cancel;
            }

            [HarmonyPatch(typeof(Player), "teleportToLocationUnsafe")]
            [HarmonyPrefix]
            private static bool TeleportToLocationUnsafe(Player __instance, ref Vector3 position, ref float yaw)
            {
                bool cancel = false;

                OnTeleport?.Invoke(__instance, ref position, ref yaw, out cancel);

                return !cancel;
            }

            [HarmonyPatch(typeof(PlayerLife), "tellDead")]
            [HarmonyPostfix]
            private static void TellDead(PlayerLife __instance, CSteamID steamID, Vector3 newRagdoll, byte newRagdollEffect)
            {
                if (__instance.channel.checkServer(steamID))
                {
                    OnDead?.Invoke(__instance.player, newRagdoll, newRagdollEffect);
                }
            }
        }

        #region Equipment

        private void OnEquipRequested(PlayerEquipment equipment, ItemJar jar, ItemAsset asset, ref bool shouldAllow)
        {
            UnturnedPlayer player = GetUnturnedPlayer(equipment.player);

            UnturnedPlayerItemEquipEvent @event = new UnturnedPlayerItemEquipEvent(player, jar, asset);

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private void OnDequipRequested(PlayerEquipment equipment, ref bool shouldAllow)
        {
            UnturnedPlayer player = GetUnturnedPlayer(equipment.player);

            UnturnedPlayerItemDequipEvent @event = new UnturnedPlayerItemDequipEvent(player);

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        #endregion

        #region Inventory

        private void OnTakeItemRequested(Player nativePlayer, byte x, byte y, uint instanceID, byte to_x, byte to_y, byte to_rot,
            byte to_page, ItemData itemData, ref bool shouldAllow)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerPickupItemEvent @event = new UnturnedPlayerPickupItemEvent(player, x, y, instanceID,
                to_x, to_y, to_rot, to_page, itemData);

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private void OnDropItemRequested(PlayerInventory inventory, Item item, ref bool shouldAllow)
        {
            UnturnedPlayer player = GetUnturnedPlayer(inventory.player);

            UnturnedPlayerDropItemEvent @event = new UnturnedPlayerDropItemEvent(player, item);

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private void OnInventoryResized(Player nativePlayer, byte page, byte width, byte height)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerInventoryResizeEvent @event = new UnturnedPlayerInventoryResizeEvent(player, page, width, height);

            Emit(@event);

            if (page == PlayerInventory.STORAGE && width == 0 && height == 0)
            {
                UnturnedPlayerCloseStorageEvent closeStorageEvent = new UnturnedPlayerCloseStorageEvent(player);

                Emit(closeStorageEvent);
            }
        }

        private void OnInventoryStored(Player nativePlayer)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerOpenStorageEvent @event = new UnturnedPlayerOpenStorageEvent(player);

            Emit(@event);
        }

        private void OnInventoryStateUpdated(Player nativePlayer)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerInventoryUpdateEvent @event = new UnturnedPlayerInventoryUpdateEvent(player);

            Emit(@event);
        }

        private void OnInventoryAdded(Player nativePlayer, byte page, byte index, ItemJar jar)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerItemAddEvent @event = new UnturnedPlayerItemAddEvent(player, page, index, jar);

            Emit(@event);
        }

        private void OnInventoryRemoved(Player nativePlayer, byte page, byte index, ItemJar jar)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerItemRemoveEvent @event = new UnturnedPlayerItemRemoveEvent(player, page, index, jar);

            Emit(@event);
        }

        private void OnInventoryUpdated(Player nativePlayer, byte page, byte index, ItemJar jar)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerItemUpdateEvent @event = new UnturnedPlayerItemUpdateEvent(player, page, index, jar);

            Emit(@event);
        }

        #endregion

        #region Stats

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

        private void OnWaterUpdated(Player nativePlayer, byte water)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerWaterUpdateEvent @event = new UnturnedPlayerWaterUpdateEvent(player, water);

            Emit(@event);
        }

        #endregion

        #region Other

        private void OnPlayerConnected(SteamPlayer steamPlayer)
        {
            UnturnedPlayer player = GetUnturnedPlayer(steamPlayer.player);

            UnturnedPlayerConnectEvent @event = new UnturnedPlayerConnectEvent(player);

            Emit(@event);
        }

        private void OnPlayerDisconnected(SteamPlayer steamPlayer)
        {
            UnturnedPlayer player = GetUnturnedPlayer(steamPlayer.player);

            UnturnedPlayerDisconnectEvent @event = new UnturnedPlayerDisconnectEvent(player);

            Emit(@event);
        }

        private void Events_OnDoDamage(Player nativePlayer, ref byte amount,
            ref EDeathCause cause, ref ELimb limb,
            ref CSteamID killer, ref bool trackKill,
            ref Vector3 ragdoll, ref ERagdollEffect ragdollEffect,
            ref bool canCauseBleeding, out bool cancel)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerDamageEvent @event = new UnturnedPlayerDamageEvent(player, amount, cause, limb, killer,
                trackKill, ragdoll, ragdollEffect, canCauseBleeding);

            Emit(@event);

            amount = @event.DamageAmount;
            cause = @event.Cause;
            limb = @event.Limb;
            killer = @event.Killer;
            trackKill = @event.TrackKill;
            ragdoll = @event.Ragdoll;
            ragdollEffect = @event.RagdollEffect;
            canCauseBleeding = @event.CanCauseBleeding;
            cancel = @event.IsCancelled;
        }

        private void Events_OnTeleport(Player nativePlayer, ref Vector3 position, ref float yaw, out bool cancel)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerTeleportEvent @event = new UnturnedPlayerTeleportEvent(player, position, yaw);

            Emit(@event);

            position = @event.Position;
            yaw = @event.Yaw;
            cancel = @event.IsCancelled;
        }

        private void Events_OnDead(Player nativePlayer, Vector3 ragdoll, byte ragdollEffect)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerDeadEvent @event = new UnturnedPlayerDeadEvent(player, ragdoll, ragdollEffect);

            Emit(@event);
        }

        private void OnPerformingAid(Player nativeInstigator, Player nativeTarget, ItemConsumeableAsset asset, ref bool shouldAllow)
        {
            UnturnedPlayer instigator = GetUnturnedPlayer(nativeInstigator);
            UnturnedPlayer target = GetUnturnedPlayer(nativeTarget);

            UnturnedPlayerPerformAidEvent @event = new UnturnedPlayerPerformAidEvent(instigator, target, asset);

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private void OnStanceUpdated(Player nativePlayer)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerStanceUpdateEvent @event = new UnturnedPlayerStanceUpdateEvent(player);

            Emit(@event);
        }

        private void OnCraftBlueprintRequested(PlayerCrafting crafting, ref ushort itemId, ref byte blueprintIndex, ref bool shouldAllow)
        {
            UnturnedPlayer player = GetUnturnedPlayer(crafting.player);

            UnturnedPlayerCraftEvent @event = new UnturnedPlayerCraftEvent(player, itemId, blueprintIndex);

            Emit(@event);

            itemId = @event.ItemId;
            blueprintIndex = @event.BlueprintIndex;
            shouldAllow = !@event.IsCancelled;
        }

        private void OnPlayerDeath(PlayerLife sender, EDeathCause cause, ELimb limb, CSteamID instigator)
        {
            UnturnedPlayer player = GetUnturnedPlayer(sender.player);

            UnturnedPlayerDeathEvent @event = new UnturnedPlayerDeathEvent(player, cause, limb, instigator);

            Emit(@event);
        }

        #endregion

        #endregion

        #region Vehicles

        private void OnEnterVehicleRequested(Player nativePlayer, InteractableVehicle vehicle, ref bool shouldAllow)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerEnterVehicleEvent @event = new UnturnedPlayerEnterVehicleEvent(player, vehicle);

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private void OnExitVehicleRequested(Player nativePlayer, InteractableVehicle vehicle, ref bool shouldAllow, ref Vector3 pendingLocation, ref float pendingYaw)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerExitVehicleEvent @event = new UnturnedPlayerExitVehicleEvent(player, vehicle, pendingLocation, pendingYaw);

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
            pendingLocation = @event.PendingLocation;
            pendingYaw = @event.PendingYaw;
        }

        private void OnSwapSeatRequested(Player nativePlayer, InteractableVehicle vehicle, ref bool shouldAllow, byte fromSeatIndex, ref byte toSeatIndex)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerSwapSeatEvent @event = new UnturnedPlayerSwapSeatEvent(player, vehicle, fromSeatIndex, toSeatIndex);

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
            toSeatIndex = @event.ToSeatIndex;
        }

        private void OnVehicleCarjacked(InteractableVehicle vehicle, Player instigatingPlayer, ref bool allow, ref Vector3 force, ref Vector3 torque)
        {
            UnturnedPlayer instigator = GetUnturnedPlayer(instigatingPlayer);

            UnturnedVehicleCarjackEvent @event = new UnturnedVehicleCarjackEvent(vehicle, instigator, force, torque);

            Emit(@event);

            allow = !@event.IsCancelled;
            force = @event.Force;
            torque = @event.Torque;
        }

        private void OnVehicleLockpicked(InteractableVehicle vehicle, Player instigatingPlayer, ref bool allow)
        {
            UnturnedPlayer instigator = GetUnturnedPlayer(instigatingPlayer);

            UnturnedVehicleLockpickEvent @event = new UnturnedVehicleLockpickEvent(vehicle, instigator);

            Emit(@event);

            allow = !@event.IsCancelled;
        }

        private void OnSiphonVehicleRequested(InteractableVehicle vehicle, Player instigatingPlayer, ref bool shouldAllow, ref ushort desiredAmount)
        {
            UnturnedPlayer instigator = GetUnturnedPlayer(instigatingPlayer);

            UnturnedVehicleSiphonEvent @event = new UnturnedVehicleSiphonEvent(vehicle, instigator, desiredAmount);

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
            desiredAmount = @event.DesiredAmount;
        }

        private void OnRepairVehicleRequested(CSteamID instigatorSteamID, InteractableVehicle vehicle, ref ushort pendingTotalHealing, ref bool shouldAllow)
        {
            UnturnedVehicleRepairEvent @event = new UnturnedVehicleRepairEvent(vehicle, instigatorSteamID, pendingTotalHealing);

            Emit(@event);

            pendingTotalHealing = @event.PendingTotalHealing;
            shouldAllow = !@event.IsCancelled;
        }

        private void OnDamageVehicleRequested(CSteamID instigatorSteamID, InteractableVehicle vehicle, ref ushort pendingTotalDamage, ref bool canRepair, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            UnturnedVehicleDamageEvent @event = new UnturnedVehicleDamageEvent(vehicle, instigatorSteamID, pendingTotalDamage, damageOrigin, canRepair);

            Emit(@event);

            pendingTotalDamage = @event.PendingTotalDamage;
            canRepair = @event.CanRepair;
            shouldAllow = !@event.IsCancelled;
        }

        private void OnDamageTireRequested(CSteamID instigatorSteamID, InteractableVehicle vehicle, int tireIndex, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            UnturnedVehicleDamageTireEvent @event = new UnturnedVehicleDamageTireEvent(vehicle, instigatorSteamID, tireIndex, damageOrigin);

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private void Events_OnVehicleExplode(InteractableVehicle vehicle, out bool cancel)
        {
            UnturnedVehicleExplodeEvent @event = new UnturnedVehicleExplodeEvent(vehicle);

            Emit(@event);

            cancel = @event.IsCancelled;
        }

        private void Events_OnVehicleSpawn(InteractableVehicle vehicle)
        {
            UnturnedVehicleSpawnEvent @event = new UnturnedVehicleSpawnEvent(vehicle);

            Emit(@event);
        }

        [HarmonyPatch]
        private class VehiclePatches
        {
            [HarmonyPatch(typeof(InteractableVehicle), "explode")]
            [HarmonyPrefix]
            private static bool Explode(InteractableVehicle __instance)
            {
                bool cancel = false;

                OnVehicleExplode?.Invoke(__instance, out cancel);

                return !cancel;
            }

            [HarmonyPatch(typeof(VehicleManager), "spawnVehicleInternal")]
            [HarmonyPostfix]
            private static void SpawnVehicleInternal(InteractableVehicle __result)
            {
                OnVehicleSpawn?.Invoke(__result);
            }
        }

        #endregion

        #region Zombies

        [HarmonyPatch]
        private class ZombiePatches
        {
            [HarmonyPatch(typeof(Zombie), "tellAlive")]
            [HarmonyPostfix]
            private static void TellAlive(Zombie __instance)
            {
                OnZombieSpawn?.Invoke(__instance);
            }

            [HarmonyPatch(typeof(ZombieManager), "addZombie")]
            [HarmonyPostfix]
            private static void AddZombie(byte bound)
            {
                Zombie zombie = ZombieManager.regions[bound].zombies.LastOrDefault();

                if (zombie != null)
                {
                    OnZombieSpawn?.Invoke(zombie);
                }
            }
        }

        #endregion

        #endregion
    }
}
