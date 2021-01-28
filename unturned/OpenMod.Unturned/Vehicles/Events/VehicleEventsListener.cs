extern alias JetBrainsAnnotations;
using HarmonyLib;
using JetBrainsAnnotations::JetBrains.Annotations;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
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
            OnVehicleLockUpdated += Events_OnVehicleLockUpdated;
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
            OnVehicleLockUpdating -= Events_OnVehicleLockUpdating;
            OnVehicleLockUpdated -= Events_OnVehicleLockUpdated;
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
                IsCancelled = allow
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

        private void Events_OnVehicleEntered(InteractableVehicle vehicle, Player nativePlayer)
        {
            var player = GetUnturnedPlayer(nativePlayer);

            var @event = new UnturnedPlayerEnteredVehicleEvent(player!, new UnturnedVehicle(vehicle));

            Emit(@event);
        }

        private void Events_OnVehicleExited(InteractableVehicle vehicle, Player nativePlayer)
        {
            var player = GetUnturnedPlayer(nativePlayer);

            var @event = new UnturnedPlayerExitedVehicleEvent(player!, new UnturnedVehicle(vehicle));

            Emit(@event);
        }

        private void Events_OnVehicleSwapped(InteractableVehicle vehicle, Player nativePlayer, byte fromSeatIndex, byte toSeatIndex)
        {
            var player = GetUnturnedPlayer(nativePlayer);

            var @event =
                new UnturnedPlayerSwappedSeatEvent(player!, new UnturnedVehicle(vehicle), fromSeatIndex, toSeatIndex);

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

        private void Events_OnVehicleLockUpdating(InteractableVehicle vehicle, Player nativePlayer, CSteamID group, bool isLocked, ref bool cancel)
        {
            var player = GetUnturnedPlayer(nativePlayer);

            var @event = new UnturnedVehicleLockUpdatingEvent(instigator: player!,
                vehicle: new UnturnedVehicle(vehicle),
                group: group == CSteamID.Nil ? null : group,
                isLocking: isLocked)
            {
                IsCancelled = cancel
            };

            Emit(@event);

            cancel = @event.IsCancelled;
        }

        private void Events_OnVehicleLockUpdated(InteractableVehicle vehicle, Player nativePlayer, CSteamID group, bool isLocked)
        {
            var player = GetUnturnedPlayer(nativePlayer);

            var @event = new UnturnedVehicleLockUpdatedEvent(
                instigator: player!,
                vehicle: new UnturnedVehicle(vehicle),
                group: group == CSteamID.Nil  ? null : group,
                isLocked: isLocked);

            Emit(@event);
        }

        private delegate void VehicleExploding(InteractableVehicle vehicle, ref bool cancel);
        private static event VehicleExploding? OnVehicleExploding;

        private delegate void VehicleSpawned(InteractableVehicle vehicle);
        private static event VehicleSpawned? OnVehicleSpawned;

        private delegate void VehicleEntered(InteractableVehicle vehicle, Player player);
        private static event VehicleEntered? OnVehicleEntered;

        private delegate void VehicleExited(InteractableVehicle vehicle, Player player);
        private static event VehicleExited? OnVehicleExited;

        private delegate void VehicleSwapped(InteractableVehicle vehicle, Player player, byte fromSeatIndex, byte toSeatIndex);
        private static event VehicleSwapped? OnVehicleSwapped;

        private delegate void VehicleStealBattery(InteractableVehicle vehicle, Player player, ref bool cancel);
        private static event VehicleStealBattery? OnVehicleStealBattery;

        private delegate void VehicleReplacingBattery(InteractableVehicle vehicle, Player player, byte amount, ref bool cancel);
        private static event VehicleReplacingBattery? OnVehicleReplacingBattery;

        private delegate void VehicleLockUpdating(InteractableVehicle vehicle, Player player, CSteamID group, bool isLocked, ref bool cancel);
        private static event VehicleLockUpdating? OnVehicleLockUpdating;

        private delegate void VehicleLockUpdated(InteractableVehicle vehicle, Player player, CSteamID group, bool isLocked);
        private static event VehicleLockUpdated? OnVehicleLockUpdated;

        [HarmonyPatch]
        [UsedImplicitly]
        internal static class VehiclePatches
        {
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

            [HarmonyPatch(typeof(InteractableVehicle), "addPlayer")]
            [HarmonyPostfix]
            [UsedImplicitly]
            public static void AddPlayer(InteractableVehicle __instance, CSteamID steamID)
            {
                var player = PlayerTool.getPlayer(steamID);

                if (player == null) return;

                OnVehicleEntered?.Invoke(__instance, player);
            }

            [HarmonyPatch(typeof(InteractableVehicle), "removePlayer")]
            [HarmonyPrefix]
            [UsedImplicitly]
            // ReSharper disable once RedundantAssignment
            public static void PreRemovePlayer(InteractableVehicle __instance, byte seatIndex, ref Player? __state)
            {
                __state = null;

                if (seatIndex < __instance.passengers?.Length)
                {
                    var passenger = __instance.passengers[seatIndex];

                    if (passenger?.player?.player != null)
                    {
                        __state = passenger.player.player;
                    }
                }
            }

            [HarmonyPatch(typeof(InteractableVehicle), "removePlayer")]
            [HarmonyPostfix]
            [UsedImplicitly]
            public static void PostRemovePlayer(InteractableVehicle __instance, Player __state)
            {
                if (__state != null)
                {
                    OnVehicleExited?.Invoke(__instance, __state);
                }
            }

            [HarmonyPatch(typeof(InteractableVehicle), "swapPlayer")]
            [HarmonyPostfix]
            [UsedImplicitly]
            public static void SwapPlayer(InteractableVehicle __instance, byte fromSeatIndex, byte toSeatIndex)
            {
                if (fromSeatIndex < __instance.passengers?.Length && toSeatIndex < __instance.passengers?.Length)
                {
                    var passenger = __instance.passengers[toSeatIndex];

                    var player = passenger?.player?.player;

                    if (player != null)
                    {
                        OnVehicleSwapped?.Invoke(__instance, player, fromSeatIndex, toSeatIndex);
                    }
                }
            }

            [HarmonyPatch(typeof(InteractableVehicle), "stealBattery")]
            [HarmonyPrefix]
            [UsedImplicitly]
            public static bool StealBattery(InteractableVehicle __instance, Player player)
            {
                var cancel = false;

                OnVehicleStealBattery?.Invoke(__instance, player, ref cancel);

                return !cancel;
            }

            [HarmonyPatch(typeof(InteractableVehicle), "replaceBattery")]
            [HarmonyPrefix]
            [UsedImplicitly]
            public static bool ReplaceBattery(InteractableVehicle __instance, Player player, byte quality)
            {
                var cancel = false;

                OnVehicleReplacingBattery?.Invoke(__instance, player, quality, ref cancel);

                return !cancel;
            }

            [HarmonyPatch(typeof(VehicleManager), "askVehicleLock")]
            [HarmonyPrefix]
            [UsedImplicitly]
            public static bool PreVehicleLock(VehicleManager __instance, CSteamID steamID)
            {
                var cancel = false;

                var player = PlayerTool.getPlayer(steamID);

                if (player == null)
                {
                    return false;
                }

                var vehicle = player.movement.getVehicle();

                if (vehicle == null || vehicle.asset == null)
                {
                    return false;
                }

                if (!vehicle.checkDriver(steamID))
                {
                    return false;
                }

                var isLocked = vehicle.isLocked;
                var flag = vehicle.asset.canBeLocked && !isLocked;

                if (isLocked == flag)
                {
                    return false;
                }

                OnVehicleLockUpdating?.Invoke(vehicle, player, player.quests.groupID, flag, ref cancel);

                return !cancel;
            }

            [HarmonyPatch(typeof(VehicleManager), "tellVehicleLock")]
            [HarmonyPostfix]
            [UsedImplicitly]
            public static void PostVehicleLock(VehicleManager __instance, uint instanceID, CSteamID owner, CSteamID group, bool locked)
            {
                var vehicle = VehicleManager.findVehicleByNetInstanceID(instanceID);
                if (vehicle == null)
                {
                    return;
                }

                var nativePlayer = PlayerTool.getPlayer(owner);
                if (nativePlayer == null)
                {
                    return;
                }

                if (locked)
                {
                    OnVehicleLockUpdated?.Invoke(vehicle, nativePlayer, group, locked);
                }
            }
        }
    }
}
