using HarmonyLib;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.UnityEngine.Extensions;
using OpenMod.Unturned.Events;
using OpenMod.Unturned.Players;
using SDG.Unturned;
using Steamworks;
using System;
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

        private delegate void VehicleExploding(InteractableVehicle vehicle, out bool cancel);
        private static event VehicleExploding OnVehicleExploding;

        private delegate void VehicleSpawned(InteractableVehicle vehicle);
        private static event VehicleSpawned OnVehicleSpawned;

        private delegate void VehicleEntered(InteractableVehicle vehicle, Player player);
        private static event VehicleEntered OnVehicleEntered;

        private delegate void VehicleExited(InteractableVehicle vehicle, Player player);
        private static event VehicleExited OnVehicleExited;

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
        }
    }
}
