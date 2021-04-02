extern alias JetBrainsAnnotations;
using System;
using HarmonyLib;
using JetBrainsAnnotations::JetBrains.Annotations;
using OpenMod.API;
using OpenMod.UnityEngine.Extensions;
using OpenMod.Unturned.Events;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace OpenMod.Unturned.Vehicles.Events
{
    [OpenModInternal]
    internal class VehicleEventsListener : UnturnedEventsListener
    {
        public VehicleEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
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
            VehicleManager.OnToggleVehicleLockRequested += VehicleManager_OnToggleVehicleLockRequested;
            VehicleManager.OnToggledVehicleLock += VehicleManager_OnToggledVehicleLock;
            InteractableVehicle.OnPassengerAdded_Global += InteractableVehicle_OnPassengerAdded_Global;
            InteractableVehicle.OnPassengerRemoved_Global += InteractableVehicle_OnPassengerRemoved_Global;
            InteractableVehicle.OnPassengerChangedSeats_Global += InteractableVehicle_OnPassengerChangedSeats_Global;
            OnVehicleExploding += Events_OnVehicleExploding;
            OnVehicleSpawned += Events_OnVehicleSpawned;
            OnVehicleStealBattery += Events_OnVehicleStealingBattery;
            OnVehicleReplacingBattery += Events_OnVehicleReplacingBattery;
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
            VehicleManager.OnToggleVehicleLockRequested -= VehicleManager_OnToggleVehicleLockRequested;
            VehicleManager.OnToggledVehicleLock -= VehicleManager_OnToggledVehicleLock;
            InteractableVehicle.OnPassengerAdded_Global -= InteractableVehicle_OnPassengerAdded_Global;
            InteractableVehicle.OnPassengerRemoved_Global -= InteractableVehicle_OnPassengerRemoved_Global;
            InteractableVehicle.OnPassengerChangedSeats_Global -= InteractableVehicle_OnPassengerChangedSeats_Global;
            OnVehicleExploding -= Events_OnVehicleExploding;
            OnVehicleSpawned -= Events_OnVehicleSpawned;
            OnVehicleStealBattery -= Events_OnVehicleStealingBattery;
            OnVehicleReplacingBattery -= Events_OnVehicleReplacingBattery;
        }

        private void InteractableVehicle_OnPassengerChangedSeats_Global(InteractableVehicle vehicle, int fromSeatIndex, int toSeatIndex)
        {
            var player = GetUnturnedPlayer(vehicle.passengers?[toSeatIndex].player);

            var @event =
                new UnturnedPlayerSwappedSeatEvent(player!, new UnturnedVehicle(vehicle), (byte)fromSeatIndex, (byte)toSeatIndex);

            Emit(@event);
        }

        private void InteractableVehicle_OnPassengerRemoved_Global(InteractableVehicle vehicle, int seat, Player nativePlayer)
        {
            var player = GetUnturnedPlayer(nativePlayer);

            var @event = new UnturnedPlayerExitedVehicleEvent(player!, new UnturnedVehicle(vehicle));

            Emit(@event);
        }

        private void InteractableVehicle_OnPassengerAdded_Global(InteractableVehicle vehicle, int seat)
        {
            var player = GetUnturnedPlayer(vehicle.passengers?[seat].player);

            var @event = new UnturnedPlayerEnteredVehicleEvent(player!, new UnturnedVehicle(vehicle));

            Emit(@event);
        }

        private void VehicleManager_OnToggledVehicleLock(InteractableVehicle vehicle)
        {
            var player = GetUnturnedPlayer(vehicle.passengers?[0].player);
            var group = vehicle.lockedGroup;
            var isLocked = vehicle.isLocked;

            var @event = new UnturnedVehicleLockUpdatedEvent(
                instigator: player!,
                vehicle: new UnturnedVehicle(vehicle),
                group: group == CSteamID.Nil ? null : group,
                isLocked: isLocked);

            Emit(@event);
        }

        private void VehicleManager_OnToggleVehicleLockRequested(InteractableVehicle vehicle, ref bool shouldallow)
        {
            var player = GetUnturnedPlayer(vehicle.passengers?[0].player);
            var group = vehicle.lockedGroup;
            var isLocked = vehicle.isLocked;

            var @event = new UnturnedVehicleLockUpdatingEvent(instigator: player!,
                vehicle: new UnturnedVehicle(vehicle),
                group: group == CSteamID.Nil ? null : group,
                isLocking: isLocked)
            {
                IsCancelled = !shouldallow
            };

            Emit(@event);

            shouldallow = !@event.IsCancelled;
        }

        private void OnEnterVehicleRequested(Player nativePlayer, InteractableVehicle vehicle, ref bool shouldAllow)
        {
            var player = GetUnturnedPlayer(nativePlayer);

            var @event = new UnturnedPlayerEnteringVehicleEvent(player!, new UnturnedVehicle(vehicle))
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private void OnExitVehicleRequested(Player nativePlayer, InteractableVehicle vehicle, ref bool shouldAllow, ref Vector3 pendingLocation, ref float pendingYaw) // lgtm [cs/too-many-ref-parameters]
        {
            var player = GetUnturnedPlayer(nativePlayer);

            var @event = new UnturnedPlayerExitingVehicleEvent(player!, new UnturnedVehicle(vehicle),
                pendingLocation.ToSystemVector(), pendingYaw)
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
            pendingLocation = @event.PendingLocation.ToUnityVector();
            pendingYaw = @event.PendingYaw;
        }

        private void OnSwapSeatRequested(Player nativePlayer, InteractableVehicle vehicle, ref bool shouldAllow, byte fromSeatIndex, ref byte toSeatIndex)
        {
            var player = GetUnturnedPlayer(nativePlayer);

            var @event =
                new UnturnedPlayerSwappingSeatEvent(player!, new UnturnedVehicle(vehicle), fromSeatIndex, toSeatIndex)
                {
                    IsCancelled = !shouldAllow
                };

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
            toSeatIndex = @event.ToSeatIndex;
        }

        private void OnVehicleCarjacked(InteractableVehicle vehicle, Player instigatingPlayer, ref bool allow, ref Vector3 force, ref Vector3 torque) // lgtm [cs/too-many-ref-parameters]
        {
            var instigator = GetUnturnedPlayer(instigatingPlayer);

            var @event = new UnturnedVehicleCarjackingEvent(new UnturnedVehicle(vehicle), instigator!,
                force.ToSystemVector(), torque.ToSystemVector())
            {
                IsCancelled = !allow
            };

            Emit(@event);

            allow = !@event.IsCancelled;
            force = @event.Force.ToUnityVector();
            torque = @event.Torque.ToUnityVector();
        }

        private void OnVehicleLockpicked(InteractableVehicle vehicle, Player instigatingPlayer, ref bool allow)
        {
            var instigator = GetUnturnedPlayer(instigatingPlayer);

            var @event = new UnturnedVehicleLockpickingEvent(new UnturnedVehicle(vehicle), instigator!)
            {
                IsCancelled = !allow
            };

            Emit(@event);

            allow = !@event.IsCancelled;
        }

        // lgtm [cs/too-many-ref-parameters]
        private void OnSiphonVehicleRequested(InteractableVehicle vehicle, Player instigatingPlayer, ref bool shouldAllow, ref ushort desiredAmount)
        {
            var instigator = GetUnturnedPlayer(instigatingPlayer);

            var @event = new UnturnedVehicleSiphoningEvent(new UnturnedVehicle(vehicle), instigator!, desiredAmount)
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
            desiredAmount = @event.DesiredAmount;
        }

        private void OnRepairVehicleRequested(CSteamID instigatorSteamId, InteractableVehicle vehicle, ref ushort pendingTotalHealing, ref bool shouldAllow)
        {
            var @event =
                new UnturnedVehicleRepairingEvent(new UnturnedVehicle(vehicle), instigatorSteamId, pendingTotalHealing)
                {
                    IsCancelled = !shouldAllow
                };

            Emit(@event);

            pendingTotalHealing = @event.PendingTotalHealing;
            shouldAllow = !@event.IsCancelled;
        }

        private void OnDamageVehicleRequested(CSteamID instigatorSteamId, InteractableVehicle vehicle, ref ushort pendingTotalDamage, ref bool canRepair, ref bool shouldAllow, EDamageOrigin damageOrigin) // lgtm [cs/too-many-ref-parameters] 
        {
            var @event = new UnturnedVehicleDamagingEvent(
                vehicle: new UnturnedVehicle(vehicle),
                instigator: instigatorSteamId == CSteamID.Nil ? null : instigatorSteamId,
                pendingTotalDamage: pendingTotalDamage,
                damageOrigin: damageOrigin,
                canRepair: canRepair)
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            pendingTotalDamage = @event.PendingTotalDamage;
            canRepair = @event.CanRepair;
            shouldAllow = !@event.IsCancelled;
        }

        private void OnDamageTireRequested(CSteamID instigatorSteamId, InteractableVehicle vehicle, int tireIndex, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            var @event = new UnturnedVehicleDamagingTireEvent(new UnturnedVehicle(vehicle), instigatorSteamId,
                tireIndex, damageOrigin)
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private void Events_OnVehicleExploding(InteractableVehicle vehicle, ref bool cancel)
        {
            var @event = new UnturnedVehicleExplodingEvent(new UnturnedVehicle(vehicle))
            {
                IsCancelled = cancel
            };

            Emit(@event);

            cancel = @event.IsCancelled;
        }

        private void Events_OnVehicleSpawned(InteractableVehicle vehicle)
        {
            var @event = new UnturnedVehicleSpawnedEvent(new UnturnedVehicle(vehicle));

            Emit(@event);
        }

        private void Events_OnVehicleStealingBattery(InteractableVehicle vehicle, Player nativePlayer, ref bool cancel)
        {
            var player = GetUnturnedPlayer(nativePlayer);

            var @event = new UnturnedVehicleStealingBatteryEvent(player!, new UnturnedVehicle(vehicle))
            {
                IsCancelled = cancel
            };

            Emit(@event);

            cancel = @event.IsCancelled;
        }

        private void Events_OnVehicleReplacingBattery(InteractableVehicle vehicle, Player nativePlayer, byte amount, ref bool cancel)
        {
            var player = GetUnturnedPlayer(nativePlayer);

            var @event = new UnturnedVehicleReplacingBatteryEvent(player!, new UnturnedVehicle(vehicle), amount)
            {
                IsCancelled = cancel
            };

            Emit(@event);

            cancel = @event.IsCancelled;
        }

        private delegate void VehicleExploding(InteractableVehicle vehicle, ref bool cancel);
        private static event VehicleExploding? OnVehicleExploding;

        private delegate void VehicleSpawned(InteractableVehicle vehicle);
        private static event VehicleSpawned? OnVehicleSpawned;

        private delegate void VehicleStealBattery(InteractableVehicle vehicle, Player player, ref bool cancel);
        private static event VehicleStealBattery? OnVehicleStealBattery;

        private delegate void VehicleReplacingBattery(InteractableVehicle vehicle, Player player, byte amount, ref bool cancel);
        private static event VehicleReplacingBattery? OnVehicleReplacingBattery;

        [HarmonyPatch]
        [UsedImplicitly]
        internal static class VehiclePatches
        {
            // ReSharper disable InconsistentNaming
            [HarmonyPatch(typeof(InteractableVehicle), "explode")]
            [HarmonyPrefix]
            [UsedImplicitly]
            public static bool Explode(InteractableVehicle __instance)
            {
                var cancel = false;

                OnVehicleExploding?.Invoke(__instance, ref cancel);

                return !cancel;
            }

            [HarmonyPatch(typeof(VehicleManager), "spawnVehicleInternal")]
            [HarmonyPostfix]
            [UsedImplicitly]
            public static void SpawnVehicleInternal(InteractableVehicle __result)
            {
                OnVehicleSpawned?.Invoke(__result);
            }

            [HarmonyPatch(typeof(InteractableVehicle), nameof(InteractableVehicle.stealBattery))]
            [HarmonyPrefix]
            [UsedImplicitly]
            public static bool StealBattery(InteractableVehicle __instance, Player player)
            {
                var cancel = false;

                OnVehicleStealBattery?.Invoke(__instance, player, ref cancel);

                return !cancel;
            }

            [HarmonyPatch(typeof(InteractableVehicle), nameof(InteractableVehicle.replaceBattery))]
            [HarmonyPrefix]
            [UsedImplicitly]
            public static bool ReplaceBattery(InteractableVehicle __instance, Player player, byte quality)
            {
                var cancel = false;

                OnVehicleReplacingBattery?.Invoke(__instance, player, quality, ref cancel);

                return !cancel;
            }

            // ReSharper restore InconsistentNaming
        }
    }
}
