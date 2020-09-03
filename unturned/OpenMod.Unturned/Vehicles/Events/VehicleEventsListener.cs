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

            UnturnedPlayerEnterVehicleEvent @event = new UnturnedPlayerEnterVehicleEvent(player, new UnturnedVehicle(vehicle));

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private void OnExitVehicleRequested(Player nativePlayer, InteractableVehicle vehicle, ref bool shouldAllow, ref Vector3 pendingLocation, ref float pendingYaw)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerExitVehicleEvent @event = new UnturnedPlayerExitVehicleEvent(player, new UnturnedVehicle(vehicle), pendingLocation.ToSystemVector(), pendingYaw);

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
            pendingLocation = @event.PendingLocation.ToUnityVector();
            pendingYaw = @event.PendingYaw;
        }

        private void OnSwapSeatRequested(Player nativePlayer, InteractableVehicle vehicle, ref bool shouldAllow, byte fromSeatIndex, ref byte toSeatIndex)
        {
            UnturnedPlayer player = GetUnturnedPlayer(nativePlayer);

            UnturnedPlayerSwapSeatEvent @event = new UnturnedPlayerSwapSeatEvent(player, new UnturnedVehicle(vehicle), fromSeatIndex, toSeatIndex);

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
            toSeatIndex = @event.ToSeatIndex;
        }

        private void OnVehicleCarjacked(InteractableVehicle vehicle, Player instigatingPlayer, ref bool allow, ref Vector3 force, ref Vector3 torque)
        {
            UnturnedPlayer instigator = GetUnturnedPlayer(instigatingPlayer);

            UnturnedVehicleCarjackEvent @event = new UnturnedVehicleCarjackEvent(new UnturnedVehicle(vehicle), instigator, force.ToSystemVector(), torque.ToSystemVector());

            Emit(@event);

            allow = !@event.IsCancelled;
            force = @event.Force.ToUnityVector();
            torque = @event.Torque.ToUnityVector();
        }

        private void OnVehicleLockpicked(InteractableVehicle vehicle, Player instigatingPlayer, ref bool allow)
        {
            UnturnedPlayer instigator = GetUnturnedPlayer(instigatingPlayer);

            UnturnedVehicleLockpickEvent @event = new UnturnedVehicleLockpickEvent(new UnturnedVehicle(vehicle), instigator);

            Emit(@event);

            allow = !@event.IsCancelled;
        }

        private void OnSiphonVehicleRequested(InteractableVehicle vehicle, Player instigatingPlayer, ref bool shouldAllow, ref ushort desiredAmount)
        {
            UnturnedPlayer instigator = GetUnturnedPlayer(instigatingPlayer);

            UnturnedVehicleSiphonEvent @event = new UnturnedVehicleSiphonEvent(new UnturnedVehicle(vehicle), instigator, desiredAmount);

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
            desiredAmount = @event.DesiredAmount;
        }

        private void OnRepairVehicleRequested(CSteamID instigatorSteamID, InteractableVehicle vehicle, ref ushort pendingTotalHealing, ref bool shouldAllow)
        {
            UnturnedVehicleRepairEvent @event = new UnturnedVehicleRepairEvent(new UnturnedVehicle(vehicle), instigatorSteamID, pendingTotalHealing);

            Emit(@event);

            pendingTotalHealing = @event.PendingTotalHealing;
            shouldAllow = !@event.IsCancelled;
        }

        private void OnDamageVehicleRequested(CSteamID instigatorSteamID, InteractableVehicle vehicle, ref ushort pendingTotalDamage, ref bool canRepair, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            UnturnedVehicleDamageEvent @event = new UnturnedVehicleDamageEvent(new UnturnedVehicle(vehicle), instigatorSteamID, pendingTotalDamage, damageOrigin, canRepair);

            Emit(@event);

            pendingTotalDamage = @event.PendingTotalDamage;
            canRepair = @event.CanRepair;
            shouldAllow = !@event.IsCancelled;
        }

        private void OnDamageTireRequested(CSteamID instigatorSteamID, InteractableVehicle vehicle, int tireIndex, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            UnturnedVehicleDamageTireEvent @event = new UnturnedVehicleDamageTireEvent(new UnturnedVehicle(vehicle), instigatorSteamID, tireIndex, damageOrigin);

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private void Events_OnVehicleExplode(InteractableVehicle vehicle, out bool cancel)
        {
            UnturnedVehicleExplodeEvent @event = new UnturnedVehicleExplodeEvent(new UnturnedVehicle(vehicle));

            Emit(@event);

            cancel = @event.IsCancelled;
        }

        private void Events_OnVehicleSpawn(InteractableVehicle vehicle)
        {
            UnturnedVehicleSpawnEvent @event = new UnturnedVehicleSpawnEvent(new UnturnedVehicle(vehicle));

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
