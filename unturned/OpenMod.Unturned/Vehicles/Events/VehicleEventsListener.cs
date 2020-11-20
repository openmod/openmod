using HarmonyLib;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.UnityEngine.Extensions;
using OpenMod.Unturned.Events;
using OpenMod.Unturned.Players;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace OpenMod.Unturned.Vehicles.Events
{
    internal class VehicleEventsListener : UnturnedEventsListener
    {
        public VehicleEventsListener(IOpenModHost openModHost,
            IEventBus eventBus,
            IUserManager userManager) : base(openModHost, eventBus, userManager)
        {

        }

        public override void Subscribe()
        {
            VehicleManager.onEnterVehicleRequested += OnEnterVehicleRequested;
            VehicleManager.onExitVehicleRequested += OnExitVehicleRequested;
            VehicleManager.onSwapSeatRequested += OnSwapSeatRequested;
            VehicleManager.onDamageTireRequested += OnDamageTireRequested;
            VehicleManager.onDamageVehicleRequested += OnDamageVehicleRequested;
            VehicleManager.onRepairVehicleRequested += OnRepairVehicleRequested;
            VehicleManager.onSiphonVehicleRequested += OnSiphonVehicleRequested;
            VehicleManager.onVehicleCarjacked += OnVehicleCarjacked;
            VehicleManager.onVehicleLockpicked += OnVehicleLockpicked;
            OnVehicleExploding += Events_OnVehicleExploding;
            OnVehicleSpawned += Events_OnVehicleSpawned;
            OnVehicleEntered += Events_OnVehicleEntered;
            OnVehicleExited += Events_OnVehicleExited;
            OnVehicleSwapped += Events_OnVehicleSwapped;
            OnVehicleStealBattery += Events_OnVehicleStealingBattery;
            OnVehicleReplacingBattery += Events_OnVehicleReplacingBattery;
            OnVehicleLockUpdating += Events_OnVehicleLockUpdating;
            OnVehicleLocked += Events_OnVehicleLocked;
        }

        public override void Unsubscribe()
        {
            VehicleManager.onEnterVehicleRequested -= OnEnterVehicleRequested;
            VehicleManager.onExitVehicleRequested -= OnExitVehicleRequested;
            VehicleManager.onSwapSeatRequested -= OnSwapSeatRequested;
            VehicleManager.onDamageTireRequested -= OnDamageTireRequested;
            VehicleManager.onDamageVehicleRequested -= OnDamageVehicleRequested;
            VehicleManager.onRepairVehicleRequested -= OnRepairVehicleRequested;
            VehicleManager.onSiphonVehicleRequested -= OnSiphonVehicleRequested;
            VehicleManager.onVehicleCarjacked -= OnVehicleCarjacked;
            VehicleManager.onVehicleLockpicked -= OnVehicleLockpicked;
            OnVehicleExploding -= Events_OnVehicleExploding;
            OnVehicleSpawned -= Events_OnVehicleSpawned;
            OnVehicleEntered -= Events_OnVehicleEntered;
            OnVehicleExited -= Events_OnVehicleExited;
            OnVehicleSwapped -= Events_OnVehicleSwapped;
            OnVehicleStealBattery -= Events_OnVehicleStealingBattery;
            OnVehicleReplacingBattery -= Events_OnVehicleReplacingBattery;
            OnVehicleLockUpdating += Events_OnVehicleLockUpdating;
            OnVehicleLocked -= Events_OnVehicleLocked;
        }

        private void OnEnterVehicleRequested(Player nativePlayer, InteractableVehicle vehicle, ref bool shouldAllow)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerEnteringVehicleEvent @event = new UnturnedPlayerEnteringVehicleEvent(player, new UnturnedVehicle(vehicle));

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private void OnExitVehicleRequested(Player nativePlayer, InteractableVehicle vehicle, ref bool shouldAllow, ref Vector3 pendingLocation, ref float pendingYaw)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerExitingVehicleEvent @event = new UnturnedPlayerExitingVehicleEvent(player, new UnturnedVehicle(vehicle), pendingLocation.ToSystemVector(), pendingYaw);

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
            pendingLocation = @event.PendingLocation.ToUnityVector();
            pendingYaw = @event.PendingYaw;
        }

        private void OnSwapSeatRequested(Player nativePlayer, InteractableVehicle vehicle, ref bool shouldAllow, byte fromSeatIndex, ref byte toSeatIndex)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerSwappingSeatEvent @event = new UnturnedPlayerSwappingSeatEvent(player, new UnturnedVehicle(vehicle), fromSeatIndex, toSeatIndex);

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
            toSeatIndex = @event.ToSeatIndex;
        }

        private void OnVehicleCarjacked(InteractableVehicle vehicle, Player instigatingPlayer, ref bool allow, ref Vector3 force, ref Vector3 torque)
        {
            UnturnedPlayer instigator = GetUnturnedPlayer(instigatingPlayer);

            UnturnedVehicleCarjackingEvent @event = new UnturnedVehicleCarjackingEvent(new UnturnedVehicle(vehicle), instigator, force.ToSystemVector(), torque.ToSystemVector());

            Emit(@event);

            allow = !@event.IsCancelled;
            force = @event.Force.ToUnityVector();
            torque = @event.Torque.ToUnityVector();
        }

        private void OnVehicleLockpicked(InteractableVehicle vehicle, Player instigatingPlayer, ref bool allow)
        {
            UnturnedPlayer instigator = GetUnturnedPlayer(instigatingPlayer);

            UnturnedVehicleLockpickingEvent @event = new UnturnedVehicleLockpickingEvent(new UnturnedVehicle(vehicle), instigator);

            Emit(@event);

            allow = !@event.IsCancelled;
        }

        private void OnSiphonVehicleRequested(InteractableVehicle vehicle, Player instigatingPlayer, ref bool shouldAllow, ref ushort desiredAmount)
        {
            UnturnedPlayer instigator = GetUnturnedPlayer(instigatingPlayer);

            UnturnedVehicleSiphoningEvent @event = new UnturnedVehicleSiphoningEvent(new UnturnedVehicle(vehicle), instigator, desiredAmount);

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
            desiredAmount = @event.DesiredAmount;
        }

        private void OnRepairVehicleRequested(CSteamID instigatorSteamID, InteractableVehicle vehicle, ref ushort pendingTotalHealing, ref bool shouldAllow)
        {
            UnturnedVehicleRepairingEvent @event = new UnturnedVehicleRepairingEvent(new UnturnedVehicle(vehicle), instigatorSteamID, pendingTotalHealing);

            Emit(@event);

            pendingTotalHealing = @event.PendingTotalHealing;
            shouldAllow = !@event.IsCancelled;
        }

        private void OnDamageVehicleRequested(CSteamID instigatorSteamID, InteractableVehicle vehicle, ref ushort pendingTotalDamage, ref bool canRepair, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            UnturnedVehicleDamagingEvent @event = new UnturnedVehicleDamagingEvent(new UnturnedVehicle(vehicle), instigatorSteamID, pendingTotalDamage, damageOrigin, canRepair);

            Emit(@event);

            pendingTotalDamage = @event.PendingTotalDamage;
            canRepair = @event.CanRepair;
            shouldAllow = !@event.IsCancelled;
        }

        private void OnDamageTireRequested(CSteamID instigatorSteamID, InteractableVehicle vehicle, int tireIndex, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            UnturnedVehicleDamagingTireEvent @event = new UnturnedVehicleDamagingTireEvent(new UnturnedVehicle(vehicle), instigatorSteamID, tireIndex, damageOrigin);

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private void Events_OnVehicleExploding(InteractableVehicle vehicle, out bool cancel)
        {
            UnturnedVehicleExplodingEvent @event = new UnturnedVehicleExplodingEvent(new UnturnedVehicle(vehicle));

            Emit(@event);

            cancel = @event.IsCancelled;
        }

        private void Events_OnVehicleSpawned(InteractableVehicle vehicle)
        {
            UnturnedVehicleSpawnedEvent @event = new UnturnedVehicleSpawnedEvent(new UnturnedVehicle(vehicle));

            Emit(@event);
        }

        private void Events_OnVehicleEntered(InteractableVehicle vehicle, Player nativePlayer)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerEnteredVehicleEvent @event =
                new UnturnedPlayerEnteredVehicleEvent(player, new UnturnedVehicle(vehicle));

            Emit(@event);
        }

        private void Events_OnVehicleExited(InteractableVehicle vehicle, Player nativePlayer)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerExitedVehicleEvent @event =
                new UnturnedPlayerExitedVehicleEvent(player, new UnturnedVehicle(vehicle));

            Emit(@event);
        }

        private void Events_OnVehicleSwapped(InteractableVehicle vehicle, Player nativePlayer, byte fromSeatIndex, byte toSeatIndex)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerSwappedSeatEvent @event =
                new UnturnedPlayerSwappedSeatEvent(player, new UnturnedVehicle(vehicle), fromSeatIndex, toSeatIndex);

            Emit(@event);
        }

        private void Events_OnVehicleStealingBattery(InteractableVehicle vehicle, Player nativePlayer, out bool cancel)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedVehicleStealingBatteryEvent @event =
                new UnturnedVehicleStealingBatteryEvent(player, new UnturnedVehicle(vehicle));

            Emit(@event);

            cancel = @event.IsCancelled;
        }

        private void Events_OnVehicleReplacingBattery(InteractableVehicle vehicle, Player nativePlayer, byte amount, out bool cancel)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedVehicleReplacingBatteryEvent @event =
                new UnturnedVehicleReplacingBatteryEvent(player, new UnturnedVehicle(vehicle), amount);

            Emit(@event);

            cancel = @event.IsCancelled;
        }

        private void Events_OnVehicleLockUpdating(InteractableVehicle vehicle, Player nativePlayer, CSteamID group, bool isLocked, out bool cancel)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedVehicleLockUpdatingEvent @event =
                new UnturnedVehicleLockUpdatingEvent(player, new UnturnedVehicle(vehicle), group, isLocked);

            Emit(@event);

            cancel = @event.IsCancelled;
        }

        private void Events_OnVehicleLocked(InteractableVehicle vehicle, Player nativePlayer, CSteamID group, bool isLocked)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedVehicleLockedEvent @event =
                new UnturnedVehicleLockedEvent(player, new UnturnedVehicle(vehicle), group, isLocked);

            Emit(@event);
        }

        private delegate void VehicleExploding(InteractableVehicle vehicle, out bool cancel);
        private static event VehicleExploding OnVehicleExploding;

        private delegate void VehicleSpawned(InteractableVehicle vehicle);
        private static event VehicleSpawned OnVehicleSpawned;

        private delegate void VehicleEntered(InteractableVehicle vehicle, Player player);
        private static event VehicleEntered OnVehicleEntered;

        private delegate void VehicleExited(InteractableVehicle vehicle, Player player);
        private static event VehicleExited OnVehicleExited;

        private delegate void VehicleSwapped(InteractableVehicle vehicle, Player player, byte fromSeatIndex, byte toSeatIndex);
        private static event VehicleSwapped OnVehicleSwapped;

        private delegate void VehicleStealBattery(InteractableVehicle vehicle, Player player, out bool cancel);
        private static event VehicleStealBattery OnVehicleStealBattery;

        private delegate void VehicleReplacingBattery(InteractableVehicle vehicle, Player player, byte amount, out bool cancel);
        private static event VehicleReplacingBattery OnVehicleReplacingBattery;

        private delegate void VehicleLockUpdating(InteractableVehicle vehicle, Player player, CSteamID group, bool isLocked, out bool cancel);
        private static event VehicleLockUpdating OnVehicleLockUpdating;

        private delegate void VehicleLocked(InteractableVehicle vehicle, Player player, CSteamID group, bool isLocked);
        private static event VehicleLocked OnVehicleLocked;

        [HarmonyPatch]
        private class VehiclePatches
        {
            [HarmonyPatch(typeof(InteractableVehicle), "explode")]
            [HarmonyPrefix]
            private static bool Explode(InteractableVehicle __instance)
            {
                bool cancel = false;

                OnVehicleExploding?.Invoke(__instance, out cancel);

                return !cancel;
            }

            [HarmonyPatch(typeof(VehicleManager), "spawnVehicleInternal")]
            [HarmonyPostfix]
            private static void SpawnVehicleInternal(InteractableVehicle __result)
            {
                OnVehicleSpawned?.Invoke(__result);
            }

            [HarmonyPatch(typeof(InteractableVehicle), "addPlayer")]
            [HarmonyPostfix]
            private static void AddPlayer(InteractableVehicle __instance, CSteamID steamID)
            {
                Player player = PlayerTool.getPlayer(steamID);

                if (player == null) return;

                OnVehicleEntered?.Invoke(__instance, player);
            }

            [HarmonyPatch(typeof(InteractableVehicle), "removePlayer")]
            [HarmonyPrefix]
            private static void PreRemovePlayer(InteractableVehicle __instance, byte seatIndex, ref Player __state)
            {
                __state = null;

                if (seatIndex < __instance.passengers?.Length)
                {
                    Passenger passenger = __instance.passengers[seatIndex];

                    if (passenger?.player?.player != null)
                    {
                        __state = passenger.player.player;
                    }
                }
            }

            [HarmonyPatch(typeof(InteractableVehicle), "removePlayer")]
            [HarmonyPostfix]
            private static void PostRemovePlayer(InteractableVehicle __instance, Player __state)
            {
                if (__state != null)
                {
                    OnVehicleExited?.Invoke(__instance, __state);
                }
            }

            [HarmonyPatch(typeof(InteractableVehicle), "swapPlayer")]
            [HarmonyPostfix]
            private static void SwapPlayer(InteractableVehicle __instance, byte fromSeatIndex, byte toSeatIndex)
            {
                if (fromSeatIndex < __instance.passengers?.Length && toSeatIndex < __instance.passengers?.Length)
                {
                    Passenger passenger = __instance.passengers[toSeatIndex];

                    Player player = passenger?.player?.player;

                    if (player != null)
                    {
                        OnVehicleSwapped?.Invoke(__instance, player, fromSeatIndex, toSeatIndex);
                    }
                }
            }

            [HarmonyPatch(typeof(InteractableVehicle), "stealBattery")]
            [HarmonyPrefix]
            private static bool StealBattery(InteractableVehicle __instance, Player player)
            {
                bool cancel = false;

                OnVehicleStealBattery?.Invoke(__instance, player, out cancel);

                return !cancel;
            }

            [HarmonyPatch(typeof(InteractableVehicle), "replaceBattery")]
            [HarmonyPrefix]
            private static bool ReplaceBattery(InteractableVehicle __instance, Player player, byte quality)
            {
                bool cancel = false;

                OnVehicleReplacingBattery?.Invoke(__instance, player, quality, out cancel);

                return !cancel;
            }

            [HarmonyPatch(typeof(VehicleManager), "askVehicleLock")]
            [HarmonyPrefix]
            private static bool PreVehicleLock(VehicleManager __instance, CSteamID steamID)
            {
                bool cancel = false;

                Player player = PlayerTool.getPlayer(steamID);
                if (player == null)
                {
                    return false;
                }
                InteractableVehicle vehicle = player.movement.getVehicle();
                if (vehicle == null || vehicle.asset == null)
                {
                    return false;
                }
                if (!vehicle.checkDriver(steamID))
                {
                    return false;
                }
                bool isLocked = vehicle.isLocked;
                bool flag = vehicle.asset.canBeLocked && !isLocked;
                if (isLocked == flag)
                {
                    return false;
                }

                OnVehicleLockUpdating?.Invoke(vehicle, player, player.quests.groupID, flag, out cancel);

                return !cancel;
            }

            [HarmonyPatch(typeof(VehicleManager), "tellVehicleLock")]
            [HarmonyPostfix]
            private static void PostVehicleLock(VehicleManager __instance, uint instanceID, CSteamID owner, CSteamID group, bool locked)
            {
                InteractableVehicle vehicle = VehicleManager.findVehicleByNetInstanceID(instanceID);
                if (vehicle is null)
                    return;

                Player nativePlayer = PlayerTool.getPlayer(owner);
                if (nativePlayer is null)
                    return;

                if (locked)
                    OnVehicleLocked?.Invoke(vehicle, nativePlayer, group, locked);
            }
        }
    }
}
