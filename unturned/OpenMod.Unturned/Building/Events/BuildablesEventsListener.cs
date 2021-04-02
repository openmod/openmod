extern alias JetBrainsAnnotations;
using HarmonyLib;
using JetBrainsAnnotations::JetBrains.Annotations;
using OpenMod.Unturned.Events;
using SDG.Unturned;
using Steamworks;
using System;
using System.Linq;
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

            OnBarricadeDeployed += Events_OnBarricadeDeployed;
            OnStructureDeployed += Events_OnStructureDeployed;

            BarricadeManager.onSalvageBarricadeRequested += OnSalvageBarricadeRequested;
            StructureManager.onSalvageStructureRequested += OnSalvageStructureRequested;

            OnBarricadeDestroyed += Events_OnBarricadeDestroyed;
            OnStructureDestroyed += Events_OnStructureDestroyed;

            BarricadeManager.onTransformRequested += OnTransformBarricadeRequested;
            StructureManager.onTransformRequested += OnTransformStructureRequested;

            OnBarricadeTransformed += Events_OnBarricadeTransformed;
            OnStructureTransformed += Events_OnStructureTransformed;
        }

        public override void Unsubscribe()
        {
            BarricadeManager.onDamageBarricadeRequested -= OnDamageBarricadeRequested;
            StructureManager.onDamageStructureRequested -= OnDamageStructureRequested;

            OnBarricadeDeployed -= Events_OnBarricadeDeployed;
            OnStructureDeployed -= Events_OnStructureDeployed;

            BarricadeManager.onSalvageBarricadeRequested -= OnSalvageBarricadeRequested;
            StructureManager.onSalvageStructureRequested -= OnSalvageStructureRequested;

            OnBarricadeDestroyed -= Events_OnBarricadeDestroyed;
            OnStructureDestroyed -= Events_OnStructureDestroyed;

            BarricadeManager.onTransformRequested -= OnTransformBarricadeRequested;
            StructureManager.onTransformRequested -= OnTransformStructureRequested;

            OnBarricadeTransformed -= Events_OnBarricadeTransformed;
            OnStructureTransformed -= Events_OnStructureTransformed;
        }

        private void OnDamageBarricadeRequested(CSteamID instigatorSteamId, Transform barricadeTransform,
            ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            if (!BarricadeManager.tryGetInfo(barricadeTransform, out _, out _, out _,
                out var index, out var region, out var drop))
            {
                return;
            }

            var data = region.barricades[index];
            var buildable = new UnturnedBarricadeBuildable(data, drop);

            var nativePlayer = PlayerTool.getPlayer(instigatorSteamId);
            var player = GetUnturnedPlayer(nativePlayer);

            var @event = pendingTotalDamage >= buildable.State.Health
                ? (UnturnedBuildableDamagingEvent)new UnturnedBarricadeDestroyingEvent(buildable, pendingTotalDamage,
                    damageOrigin, player!, instigatorSteamId)
                : new UnturnedBarricadeDamagingEvent(buildable, pendingTotalDamage, damageOrigin, player!,
                    instigatorSteamId);

            @event.IsCancelled = !shouldAllow;

            Emit(@event);

            pendingTotalDamage = @event.DamageAmount;
            shouldAllow = !@event.IsCancelled;
        }

        private void OnDamageStructureRequested(CSteamID instigatorSteamId, Transform structureTransform,
            ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            if (!StructureManager.tryGetInfo(structureTransform, out _, out _, out var index,
                out var region, out var drop))
            {
                return;
            }

            var data = region.structures[index];
            var buildable = new UnturnedStructureBuildable(data, drop);

            var nativePlayer = PlayerTool.getPlayer(instigatorSteamId);
            var player = GetUnturnedPlayer(nativePlayer);

            var @event = pendingTotalDamage >= buildable.State.Health
                ? (UnturnedBuildableDamagingEvent)new UnturnedStructureDestroyingEvent(buildable,
                    pendingTotalDamage, damageOrigin, player!, instigatorSteamId)
                : new UnturnedStructureDamagingEvent(buildable, pendingTotalDamage, damageOrigin, player!,
                    instigatorSteamId);

            @event.IsCancelled = !shouldAllow;

            Emit(@event);

            pendingTotalDamage = @event.DamageAmount;
            shouldAllow = !@event.IsCancelled;
        }

        private void Events_OnBarricadeDeployed(BarricadeData data, BarricadeDrop drop)
        {
            var @event = new UnturnedBarricadeDeployedEvent(new UnturnedBarricadeBuildable(data, drop));

            Emit(@event);
        }

        private void Events_OnStructureDeployed(StructureData data, StructureDrop drop)
        {
            var @event = new UnturnedStructureDeployedEvent(new UnturnedStructureBuildable(data, drop));

            Emit(@event);
        }

        private void OnSalvageBarricadeRequested(CSteamID steamId, byte x, byte y, ushort plant, ushort index, ref bool shouldAllow)
        {
            if (!BarricadeManager.tryGetRegion(x, y, plant, out var region))
            {
                return;
            }

            var data = region.barricades[index];
            var drop = region.drops[index];

            var nativePlayer = PlayerTool.getPlayer(steamId);
            var player = GetUnturnedPlayer(nativePlayer);

            var @event = new UnturnedBarricadeSalvagingEvent(new UnturnedBarricadeBuildable(data, drop), player!, steamId)
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private void OnSalvageStructureRequested(CSteamID steamId, byte x, byte y, ushort index, ref bool shouldAllow)
        {
            if (!StructureManager.tryGetRegion(x, y, out var region))
            {
                return;
            }

            var data = region.structures[index];
            var drop = region.drops[index];

            var nativePlayer = PlayerTool.getPlayer(steamId);
            var player = GetUnturnedPlayer(nativePlayer);

            var @event = new UnturnedStructureSalvagingEvent(new UnturnedStructureBuildable(data, drop), player!, steamId)
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private void Events_OnBarricadeDestroyed(BarricadeData data, BarricadeDrop drop)
        {
            var @event = new UnturnedBarricadeDestroyedEvent(new UnturnedBarricadeBuildable(data, drop));

            Emit(@event);
        }

        private void Events_OnStructureDestroyed(StructureData data, StructureDrop drop)
        {
            var @event = new UnturnedStructureDestroyedEvent(new UnturnedStructureBuildable(data, drop));

            Emit(@event);
        }

        private void OnTransformBarricadeRequested(CSteamID instigator, byte x, byte y, ushort plant, uint instanceId, // lgtm [cs/too-many-ref-parameters]
            ref Vector3 point, ref byte angleX, ref byte angleY, ref byte angleZ, ref bool shouldAllow)
        {
            if (!BarricadeManager.tryGetRegion(x, y, plant, out var region))
            {
                return;
            }

            var index = region.barricades.FindIndex(k => k.instanceID == instanceId);
            var data = region.barricades[index];
            var drop = region.drops[index];

            var nativePlayer = PlayerTool.getPlayer(instigator);
            var player = GetUnturnedPlayer(nativePlayer);

            var rot = Quaternion.Euler(angleX * 2, angleY * 2, angleZ * 2); // lgtm [cs/loss-of-precision]

            var @event = new UnturnedBarricadeTransformingEvent(
                new UnturnedBarricadeBuildable(data, drop), player!, instigator, point, rot)
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
            point = @event.Point;

            var eulerAngles = @event.Rotation.eulerAngles;

            angleX = MeasurementTool.angleToByte(Mathf.RoundToInt(eulerAngles.x / 2f) * 2); // lgtm [cs/loss-of-precision]
            angleY = MeasurementTool.angleToByte(Mathf.RoundToInt(eulerAngles.y / 2f) * 2); // lgtm [cs/loss-of-precision]
            angleZ = MeasurementTool.angleToByte(Mathf.RoundToInt(eulerAngles.z / 2f) * 2); // lgtm [cs/loss-of-precision]
        }

        private void OnTransformStructureRequested(CSteamID instigator, byte x, byte y, uint instanceId, // lgtm [cs/too-many-ref-parameters]
            ref Vector3 point, ref byte angleX, ref byte angleY, ref byte angleZ, ref bool shouldAllow)
        {
            if (!StructureManager.tryGetRegion(x, y, out var region))
            {
                return;
            }

            var index = region.structures.FindIndex(k => k.instanceID == instanceId);
            var data = region.structures[index];
            var drop = region.drops[index];

            var nativePlayer = PlayerTool.getPlayer(instigator);
            var player = GetUnturnedPlayer(nativePlayer);

            var rot = Quaternion.Euler(angleX * 2, angleY * 2, angleZ * 2); // lgtm [cs/loss-of-precision]

            var @event = new UnturnedStructureTransformingEvent(
                new UnturnedStructureBuildable(data, drop), player!, instigator, point, rot)
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
            point = @event.Point;

            var eulerAngles = @event.Rotation.eulerAngles;

            angleX = MeasurementTool.angleToByte(Mathf.RoundToInt(eulerAngles.x / 2f) * 2); // lgtm [cs/loss-of-precision]
            angleY = MeasurementTool.angleToByte(Mathf.RoundToInt(eulerAngles.y / 2f) * 2); // lgtm [cs/loss-of-precision]
            angleZ = MeasurementTool.angleToByte(Mathf.RoundToInt(eulerAngles.z / 2f) * 2); // lgtm [cs/loss-of-precision]
        }

        private void Events_OnBarricadeTransformed(BarricadeData data, BarricadeDrop drop, CSteamID instigatorSteamId)
        {
            var nativePlayer = PlayerTool.getPlayer(instigatorSteamId);
            var player = GetUnturnedPlayer(nativePlayer);

            var @event = new UnturnedBarricadeTransformedEvent(new UnturnedBarricadeBuildable(data, drop),
                instigatorSteamId, player);

            Emit(@event);
        }

        private void Events_OnStructureTransformed(StructureData data, StructureDrop drop, CSteamID instigatorSteamId)
        {
            var nativePlayer = PlayerTool.getPlayer(instigatorSteamId);
            var player = GetUnturnedPlayer(nativePlayer);

            var @event = new UnturnedStructureTransformedEvent(new UnturnedStructureBuildable(data, drop),
                instigatorSteamId, player);

            Emit(@event);
        }

        private delegate void BarricadeDeployed(BarricadeData data, BarricadeDrop drop);
        private static event BarricadeDeployed? OnBarricadeDeployed;

        private delegate void StructureDeployed(StructureData data, StructureDrop drop);
        private static event StructureDeployed? OnStructureDeployed;

        private delegate void BarricadeDestroyed(BarricadeData data, BarricadeDrop drop);
        private static event BarricadeDestroyed? OnBarricadeDestroyed;

        private delegate void StructureDestroyed(StructureData data, StructureDrop drop);
        private static event StructureDestroyed? OnStructureDestroyed;

        private delegate void BarricadeTransformed(BarricadeData data, BarricadeDrop drop, CSteamID instigatorSteamId);
        private static event BarricadeTransformed? OnBarricadeTransformed;

        private delegate void StructureTransformed(StructureData data, StructureDrop drop, CSteamID instigatorSteamId);
        private static event StructureTransformed? OnStructureTransformed;

        private static CSteamID s_CurrentTransformingPlayerId = CSteamID.Nil;

        [UsedImplicitly]
        [HarmonyPatch]
        private static class Patches
        {
            [UsedImplicitly]
            [HarmonyPatch(typeof(BarricadeManager), "dropBarricadeIntoRegionInternal")]
            [HarmonyPostfix]
            private static void DropBarricade(BarricadeRegion region, BarricadeData data, ref Transform result, ref uint instanceID)
            {
                if (result == null)
                {
                    return;
                }

                var drop = region.drops.LastOrDefault();
                if (drop?.instanceID == instanceID)
                {
                    OnBarricadeDeployed?.Invoke(data, drop);
                }
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(StructureManager), nameof(StructureManager.dropReplicatedStructure))]
            [HarmonyPostfix]
            private static void DropStructure(bool __result, uint ___instanceCount, Vector3 point)
            {
                if (!__result)
                {
                    return;
                }

                if (!Regions.tryGetCoordinate(point, out var x, out var y))
                {
                    return;
                }
                if (!StructureManager.tryGetRegion(x, y, out var region))
                {
                    return;
                }

                var data = region.structures.LastOrDefault();
                var drop = region.drops.LastOrDefault();

                if (data?.instanceID == ___instanceCount && drop?.instanceID == ___instanceCount)
                {
                    OnStructureDeployed?.Invoke(data, drop);
                }
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(BarricadeManager), nameof(BarricadeManager.destroyBarricade))]
            [HarmonyPrefix]
            private static void DestroyBarricade(BarricadeRegion region, ushort index)
            {
                ThreadUtil.assertIsGameThread();

                var data = region.barricades[index];
                var drop = region.drops[index];

                OnBarricadeDestroyed?.Invoke(data, drop);
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(StructureManager), nameof(StructureManager.destroyStructure))]
            [HarmonyPrefix]
            private static void DestroyStructure(StructureRegion region, ushort index)
            {
                ThreadUtil.assertIsGameThread();

                var data = region.structures[index];
                var drop = region.drops[index];

                OnStructureDestroyed?.Invoke(data, drop);
            }

            [HarmonyPatch(typeof(BarricadeManager), nameof(BarricadeManager.ReceiveTransformBarricadeRequest))]
            [HarmonyPrefix]
            private static void PreAskTransformBarricade(in ServerInvocationContext context)
            {
                s_CurrentTransformingPlayerId = context.GetCallingPlayer()?.playerID.steamID ?? CSteamID.Nil;
            }

            [HarmonyPatch(typeof(BarricadeManager), nameof(BarricadeManager.ReceiveTransformBarricadeRequest))]
            [HarmonyPostfix]
            private static void PostAskTransformBarricade()
            {
                s_CurrentTransformingPlayerId = CSteamID.Nil;
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(StructureManager), nameof(StructureManager.ReceiveTransformStructureRequest))]
            [HarmonyPrefix]
            private static void PreAskTransformStructure(in ServerInvocationContext context)
            {
                s_CurrentTransformingPlayerId = context.GetCallingPlayer()?.playerID.steamID ?? CSteamID.Nil;
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(StructureManager), nameof(StructureManager.ReceiveTransformStructureRequest))]
            [HarmonyPostfix]
            private static void PostAskTransformStructure()
            {
                s_CurrentTransformingPlayerId = CSteamID.Nil;
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(BarricadeManager), nameof(BarricadeManager.ReceiveTransformBarricade))]
            [HarmonyPostfix]
            private static void ReceiveTransformBarricade(byte x, byte y, ushort plant, uint instanceID)
            {
                ThreadUtil.assertIsGameThread();

                if (!BarricadeManager.tryGetRegion(x, y, plant, out var region))
                    return;

                var index = region.barricades.FindIndex(k => k.instanceID == instanceID);

                if (index < 0)
                    return;

                var data = region.barricades[index];
                var drop = region.drops[index];

                OnBarricadeTransformed?.Invoke(data, drop, s_CurrentTransformingPlayerId);
            }

            [UsedImplicitly]
            [HarmonyPatch(typeof(StructureManager), nameof(StructureManager.ReceiveTransformStructure))]
            [HarmonyPostfix]
            private static void ReceiveTransformStructure(byte x, byte y, uint instanceID)
            {
                ThreadUtil.assertIsGameThread();

                if (!StructureManager.tryGetRegion(x, y, out var region))
                    return;

                var index = region.structures.FindIndex(k => k.instanceID == instanceID);

                if (index < 0)
                    return;

                var data = region.structures[index];
                var drop = region.drops[index];

                OnStructureTransformed?.Invoke(data, drop, s_CurrentTransformingPlayerId);
            }
        }
    }
}
