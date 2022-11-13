extern alias JetBrainsAnnotations;
using System;
using System.Reflection;
using HarmonyLib;
using JetBrainsAnnotations::JetBrains.Annotations;
using OpenMod.Unturned.Events;
using OpenMod.Unturned.Patching;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace OpenMod.Unturned.Building.Events
{
    [UsedImplicitly]
    internal class BuildablesEventsListener : UnturnedEventsListener
    {
        public BuildablesEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override void Subscribe()
        {
            BarricadeManager.onDamageBarricadeRequested += OnDamageBarricadeRequested;
            StructureManager.onDamageStructureRequested += OnDamageStructureRequested;

            BarricadeManager.onDeployBarricadeRequested += OnDeployBarricadeRequested;
            StructureManager.onDeployStructureRequested += OnDeployStructureRequested;

            InteractableFarm.OnHarvestRequested_Global += OnHarvestPlantRequested;

            BarricadeManager.onOpenStorageRequested += OnOpenStorageRequested;

            BarricadeManager.onModifySignRequested += OnModifySignRequested;

            BarricadeManager.onBarricadeSpawned += Events_OnBarricadeDeployed;
            StructureManager.onStructureSpawned += Events_OnStructureDeployed;

            BarricadeDrop.OnSalvageRequested_Global += OnSalvageBarricadeRequested;
            StructureDrop.OnSalvageRequested_Global += OnSalvageStructureRequested;

            OnBarricadeDestroying += Events_OnBarricadeDestroyed;
            OnStructureDestroying += Events_OnStructureDestroyed;

            BarricadeManager.onTransformRequested += OnTransformBarricadeRequested;
            StructureManager.onTransformRequested += OnTransformStructureRequested;

            OnBarricadeTransformed += Events_OnBarricadeTransformed;
            OnStructureTransformed += Events_OnStructureTransformed;
        }

        public override void Unsubscribe()
        {
            BarricadeManager.onDamageBarricadeRequested -= OnDamageBarricadeRequested;
            StructureManager.onDamageStructureRequested -= OnDamageStructureRequested;

            BarricadeManager.onDeployBarricadeRequested -= OnDeployBarricadeRequested;
            StructureManager.onDeployStructureRequested -= OnDeployStructureRequested;

            InteractableFarm.OnHarvestRequested_Global -= OnHarvestPlantRequested;

            BarricadeManager.onOpenStorageRequested -= OnOpenStorageRequested;

            BarricadeManager.onModifySignRequested -= OnModifySignRequested;

            BarricadeManager.onBarricadeSpawned -= Events_OnBarricadeDeployed;
            StructureManager.onStructureSpawned -= Events_OnStructureDeployed;

            BarricadeDrop.OnSalvageRequested_Global -= OnSalvageBarricadeRequested;
            StructureDrop.OnSalvageRequested_Global -= OnSalvageStructureRequested;

            OnBarricadeDestroying -= Events_OnBarricadeDestroyed;
            OnStructureDestroying -= Events_OnStructureDestroyed;

            BarricadeManager.onTransformRequested -= OnTransformBarricadeRequested;
            StructureManager.onTransformRequested -= OnTransformStructureRequested;

            OnBarricadeTransformed -= Events_OnBarricadeTransformed;
            OnStructureTransformed -= Events_OnStructureTransformed;
        }

        private void OnDamageBarricadeRequested(CSteamID instigatorSteamId, Transform barricadeTransform,
            ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            var drop = BarricadeManager.FindBarricadeByRootTransform(barricadeTransform);
            if (drop == null)
            {
                return;
            }

            var buildable = new UnturnedBarricadeBuildable(drop);

            var nativePlayer = PlayerTool.getPlayer(instigatorSteamId);
            var player = GetUnturnedPlayer(nativePlayer);

            var @event = pendingTotalDamage >= buildable.State.Health
                ? (UnturnedBuildableDamagingEvent)new UnturnedBarricadeDestroyingEvent(buildable, pendingTotalDamage,
                    damageOrigin, player)
                : new UnturnedBarricadeDamagingEvent(buildable, pendingTotalDamage, damageOrigin, player);

            @event.IsCancelled = !shouldAllow;

            Emit(@event);

            pendingTotalDamage = @event.DamageAmount;
            shouldAllow = !@event.IsCancelled;
        }

        private void OnDamageStructureRequested(CSteamID instigatorSteamId, Transform structureTransform,
            ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            var drop = StructureManager.FindStructureByRootTransform(structureTransform);
            if (drop == null)
            {
                return;
            }

            var buildable = new UnturnedStructureBuildable(drop);

            var nativePlayer = PlayerTool.getPlayer(instigatorSteamId);
            var player = GetUnturnedPlayer(nativePlayer);

            var @event = pendingTotalDamage >= buildable.State.Health
                ? (UnturnedBuildableDamagingEvent)new UnturnedStructureDestroyingEvent(buildable,
                    pendingTotalDamage, damageOrigin, player)
                : new UnturnedStructureDamagingEvent(buildable, pendingTotalDamage, damageOrigin, player);

            @event.IsCancelled = !shouldAllow;

            Emit(@event);

            pendingTotalDamage = @event.DamageAmount;
            shouldAllow = !@event.IsCancelled;
        }

        private void OnDeployBarricadeRequested(Barricade barricade, ItemBarricadeAsset asset, Transform hit, ref Vector3 point,
            ref float angleX, ref float angleY, ref float angleZ, ref ulong owner, ref ulong @group, ref bool shouldAllow)
        {
            var rot = Quaternion.Euler(angleX, angleY, angleZ);

            var @event = new UnturnedBarricadeDeployingEvent(asset, hit, point, rot, owner, group)
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            point = @event.Point;

            var eulerAngles = @event.Rotation.eulerAngles;

            angleX = eulerAngles.x;
            angleY = eulerAngles.y;
            angleZ = eulerAngles.z;

            owner = @event.Owner;
            group = @event.Group;

            shouldAllow = !@event.IsCancelled;
        }

        private void OnDeployStructureRequested(Structure structure, ItemStructureAsset asset, // lgtm [cs/too-many-ref-parameters]
            ref Vector3 point, ref float angleX, ref float angleY, ref float angleZ, ref ulong owner, ref ulong @group, ref bool shouldAllow)
        {
            var rot = Quaternion.Euler(angleX, angleY, angleZ);

            var @event = new UnturnedStructureDeployingEvent(asset, point, rot, owner, group)
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            point = @event.Point;

            var eulerAngles = @event.Rotation.eulerAngles;

            angleX = eulerAngles.x;
            angleY = eulerAngles.y;
            angleZ = eulerAngles.z;

            owner = @event.Owner;
            group = @event.Group;

            shouldAllow = !@event.IsCancelled;
        }

        private void OnHarvestPlantRequested(InteractableFarm harvestable, SteamPlayer instigatorPlayer, ref bool shouldAllow)
        {
            var drop = BarricadeManager.FindBarricadeByRootTransform(harvestable.transform);
            if (drop == null)
            {
                return;
            }

            var player = GetUnturnedPlayer(instigatorPlayer);

            var @event = new UnturnedPlantHarvestingEvent(new UnturnedBarricadeBuildable(drop), player)
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private void OnOpenStorageRequested(CSteamID steamId, InteractableStorage storage, ref bool shouldAllow)
        {
            var drop = BarricadeManager.FindBarricadeByRootTransform(storage.transform);
            if (drop == null)
            {
                return;
            }

            var buildable = new UnturnedBarricadeBuildable(drop);

            var nativePlayer = PlayerTool.getPlayer(steamId);
            var player = GetUnturnedPlayer(nativePlayer);

            var @event = new UnturnedStorageOpeningEvent(buildable, player)
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private void OnModifySignRequested(CSteamID steamId, InteractableSign sign,  // lgtm [cs/too-many-ref-parameters]
            ref string text, ref bool shouldAllow)
        {
            var drop = BarricadeManager.FindBarricadeByRootTransform(sign.transform);
            if (drop == null)
            {
                return;
            }

            var buildable = new UnturnedBarricadeBuildable(drop);

            var nativePlayer = PlayerTool.getPlayer(steamId);
            var player = GetUnturnedPlayer(nativePlayer);

            var @event = new UnturnedSignModifyingEvent(buildable, player, text)
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            text = @event.Text;

            shouldAllow = !@event.IsCancelled;
        }

        private void Events_OnBarricadeDeployed(BarricadeRegion region, BarricadeDrop drop)
        {
            var @event = new UnturnedBarricadeDeployedEvent(new UnturnedBarricadeBuildable(drop));

            Emit(@event);
        }

        private void Events_OnStructureDeployed(StructureRegion region, StructureDrop drop)
        {
            var @event = new UnturnedStructureDeployedEvent(new UnturnedStructureBuildable(drop));

            Emit(@event);
        }

        private void OnSalvageBarricadeRequested(BarricadeDrop drop, SteamPlayer instigator, ref bool shouldAllow)
        {
            var player = GetUnturnedPlayer(instigator);

            var @event = new UnturnedBarricadeSalvagingEvent(new UnturnedBarricadeBuildable(drop), player!)
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private void OnSalvageStructureRequested(StructureDrop drop, SteamPlayer instigator, ref bool shouldAllow)
        {
            var player = GetUnturnedPlayer(instigator);

            var @event = new UnturnedStructureSalvagingEvent(new UnturnedStructureBuildable(drop), player!)
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private void Events_OnBarricadeDestroyed(BarricadeDrop drop)
        {
            var @event = new UnturnedBarricadeDestroyedEvent(new UnturnedBarricadeBuildable(drop));

            Emit(@event);
        }

        private void Events_OnStructureDestroyed(StructureDrop drop)
        {
            var @event = new UnturnedStructureDestroyedEvent(new UnturnedStructureBuildable(drop));

            Emit(@event);
        }

        private void OnTransformBarricadeRequested(CSteamID instigator, byte x, byte y, ushort plant, uint instanceId,
            ref Vector3 point, ref byte angleX, ref byte angleY, ref byte angleZ, ref bool shouldAllow)
        {
            if (!BarricadeManager.tryGetRegion(x, y, plant, out var region))
            {
                return;
            }

            var drop = region.drops.Find(x => x.instanceID == instanceId);
            if (drop == null)
            {
                return;
            }

            var nativePlayer = PlayerTool.getPlayer(instigator);
            var player = GetUnturnedPlayer(nativePlayer);

            var rot = Quaternion.Euler(angleX * 2f, angleY * 2f, angleZ * 2f);

            var @event = new UnturnedBarricadeTransformingEvent(
                new UnturnedBarricadeBuildable(drop), player!, point, rot)
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
            point = @event.Point;

            var eulerAngles = @event.Rotation.eulerAngles;

            angleX = MeasurementTool.angleToByte(Mathf.RoundToInt(eulerAngles.x / 2f) * 2f);
            angleY = MeasurementTool.angleToByte(Mathf.RoundToInt(eulerAngles.y / 2f) * 2f);
            angleZ = MeasurementTool.angleToByte(Mathf.RoundToInt(eulerAngles.z / 2f) * 2f);
        }

        private void OnTransformStructureRequested(CSteamID instigator, byte x, byte y, uint instanceId,
            ref Vector3 point, ref byte angleX, ref byte angleY, ref byte angleZ, ref bool shouldAllow)
        {
            if (!StructureManager.tryGetRegion(x, y, out var region))
            {
                return;
            }

            var drop = region.drops.Find(x => x.instanceID == instanceId);
            if (drop == null)
            {
                return;
            }

            var nativePlayer = PlayerTool.getPlayer(instigator);
            var player = GetUnturnedPlayer(nativePlayer);

            var rot = Quaternion.Euler(angleX * 2f, angleY * 2f, angleZ * 2f);

            var @event = new UnturnedStructureTransformingEvent(
                new UnturnedStructureBuildable(drop), player!, point, rot)
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
            point = @event.Point;

            var eulerAngles = @event.Rotation.eulerAngles;

            angleX = MeasurementTool.angleToByte(Mathf.RoundToInt(eulerAngles.x / 2f) * 2f);
            angleY = MeasurementTool.angleToByte(Mathf.RoundToInt(eulerAngles.y / 2f) * 2f);
            angleZ = MeasurementTool.angleToByte(Mathf.RoundToInt(eulerAngles.z / 2f) * 2f);
        }

        private void Events_OnBarricadeTransformed(BarricadeDrop drop)
        {
            var player = GetUnturnedPlayer(s_CurrentTransformingPlayer);

            var @event = new UnturnedBarricadeTransformedEvent(new UnturnedBarricadeBuildable(drop), player);

            Emit(@event);
        }

        private void Events_OnStructureTransformed(StructureDrop drop)
        {
            var player = GetUnturnedPlayer(s_CurrentTransformingPlayer);

            var @event = new UnturnedStructureTransformedEvent(new UnturnedStructureBuildable(drop), player);

            Emit(@event);
        }

#pragma warning disable RCS1213 // Remove unused member declaration.
        private delegate void BarricadeDestroyed(BarricadeDrop drop);
        private static event BarricadeDestroyed? OnBarricadeDestroying;

        private delegate void StructureDestroyed(StructureDrop drop);
        private static event StructureDestroyed? OnStructureDestroying;

        private delegate void BarricadeTransformed(BarricadeDrop drop);
        private static event BarricadeTransformed? OnBarricadeTransformed;

        private delegate void StructureTransformed(StructureDrop drop);
        private static event StructureTransformed? OnStructureTransformed;
#pragma warning restore RCS1213 // Remove unused member declaration.

        private static Player? s_CurrentTransformingPlayer;

        [UsedImplicitly]
        [HarmonyPatch]
        private static class Patches
        {
            [HarmonyCleanup]
            public static Exception? Cleanup(Exception ex, MethodBase original)
            {
                HarmonyExceptionHandler.ReportCleanupException(typeof(Patches), ex, original);
                return null;
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(BarricadeManager), nameof(BarricadeManager.ReceiveDestroyBarricade))]
            [HarmonyPrefix]
            private static void BarricadeDestroyed(NetId netId)
            {
                var barricade = NetIdRegistry.Get<BarricadeDrop>(netId);
                if (barricade == null)
                {
                    return;
                }

                OnBarricadeDestroying?.Invoke(barricade);
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(StructureManager), nameof(StructureManager.ReceiveDestroyStructure))]
            [HarmonyPrefix]
            private static void StructureDestroyed(NetId netId)
            {
                var structure = NetIdRegistry.Get<StructureDrop>(netId);
                if (structure == null)
                {
                    return;
                }

                OnStructureDestroying?.Invoke(structure);
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(BarricadeDrop), nameof(BarricadeDrop.ReceiveTransformRequest))]
            [HarmonyPrefix]
            private static void PreAskTransformBarricadeRequest(in ServerInvocationContext context)
            {
                s_CurrentTransformingPlayer = context.GetPlayer();
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(BarricadeDrop), nameof(BarricadeDrop.ReceiveTransformRequest))]
            [HarmonyPostfix]
            private static void PostAskTransformBarricadeRequest()
            {
                s_CurrentTransformingPlayer = null;
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(StructureDrop), nameof(StructureDrop.ReceiveTransformRequest))]
            [HarmonyPrefix]
            private static void PreAskTransformStructureRequest(in ServerInvocationContext context)
            {
                s_CurrentTransformingPlayer = context.GetPlayer();
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(StructureDrop), nameof(StructureDrop.ReceiveTransformRequest))]
            [HarmonyPostfix]
            private static void PostAskTransformStructureRequest()
            {
                s_CurrentTransformingPlayer = null;
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(BarricadeDrop), nameof(BarricadeDrop.ReceiveTransform))]
            [HarmonyPostfix]
            private static void ReceiveTransformBarricade(BarricadeDrop __instance)
            {
                OnBarricadeTransformed?.Invoke(__instance);
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(StructureDrop), nameof(StructureDrop.ReceiveTransform))]
            [HarmonyPostfix]
            private static void ReceiveTransformBarricade(StructureDrop __instance)
            {
                OnStructureTransformed?.Invoke(__instance);
            }
        }
    }
}
