using HarmonyLib;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
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
            OnVehicleExplode += Events_OnVehicleExplode;
            OnVehicleSpawn += Events_OnVehicleSpawn;
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
            OnVehicleExplode -= Events_OnVehicleExplode;
            OnVehicleSpawn -= Events_OnVehicleSpawn;
        }

        private delegate void VehicleExplode(InteractableVehicle vehicle, out bool cancel);
        private static event VehicleExplode OnVehicleExplode;

        private delegate void VehicleSpawn(InteractableVehicle vehicle);
        private static event VehicleSpawn OnVehicleSpawn;

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
    }
}
