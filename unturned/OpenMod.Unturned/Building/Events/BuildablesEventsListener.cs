using HarmonyLib;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.Unturned.Events;
using SDG.Unturned;
using Steamworks;
using System.Linq;
using UnityEngine;
// ReSharper disable RedundantAssignment
// ReSharper disable DelegateSubtraction
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace OpenMod.Unturned.Building.Events
{
    // ReSharper disable once UnusedMember.Global
    internal class BuildablesEventsListener : UnturnedEventsListener
    {
        public BuildablesEventsListener(IOpenModHost openModHost,
            IEventBus eventBus,
            IUserManager userManager) : base(openModHost, eventBus, userManager)
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

        private void OnDamageBarricadeRequested(CSteamID instigatorSteamId, Transform barricadeTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
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
                ? (UnturnedBuildableDamagingEvent) new UnturnedBarricadeDestroyingEvent(buildable, pendingTotalDamage,
                    damageOrigin, player, instigatorSteamId)
                : new UnturnedBarricadeDamagingEvent(buildable, pendingTotalDamage, damageOrigin, player,
                    instigatorSteamId);

            @event.IsCancelled = !shouldAllow;

            Emit(@event);

            pendingTotalDamage = @event.DamageAmount;
            shouldAllow = !@event.IsCancelled;
        }

        private void OnDamageStructureRequested(CSteamID instigatorSteamId, Transform structureTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            if (!StructureManager.tryGetInfo(structureTransform, out _, out _, out var index,
                out var region, out var drop))
            {
                return;
            }
            {
                var data = region.structures[index];
                var buildable = new UnturnedStructureBuildable(data, drop);

                var nativePlayer = PlayerTool.getPlayer(instigatorSteamId);
                var player = GetUnturnedPlayer(nativePlayer);

                var @event = pendingTotalDamage >= buildable.State.Health
                    ? (UnturnedBuildableDamagingEvent) new UnturnedStructureDestroyingEvent(buildable,
                        pendingTotalDamage, damageOrigin, player, instigatorSteamId)
                    : new UnturnedStructureDamagingEvent(buildable, pendingTotalDamage, damageOrigin, player,
                        instigatorSteamId);

                @event.IsCancelled = !shouldAllow;

                Emit(@event);

                pendingTotalDamage = @event.DamageAmount;
                shouldAllow = !@event.IsCancelled;
            }
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

            var @event = new UnturnedBarricadeSalvagingEvent(new UnturnedBarricadeBuildable(data, drop), player)
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

            var @event = new UnturnedStructureSalvagingEvent(new UnturnedStructureBuildable(data, drop), player)
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

        private void OnTransformBarricadeRequested(CSteamID instigator, byte x, byte y, ushort plant, uint instanceId,
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

            var @event = new UnturnedBarricadeTransformingEvent(
                new UnturnedBarricadeBuildable(data, drop), player, instigator, point,
                Quaternion.Euler(angleX * 2, angleY * 2, angleZ * 2))
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
            point = @event.Point;

            var eulerAngles = @event.Rotation.eulerAngles;

            angleX = MeasurementTool.angleToByte(Mathf.RoundToInt(eulerAngles.x / 2f) * 2);
            angleY = MeasurementTool.angleToByte(Mathf.RoundToInt(eulerAngles.y / 2f) * 2);
            angleZ = MeasurementTool.angleToByte(Mathf.RoundToInt(eulerAngles.z / 2f) * 2);
        }

        private void OnTransformStructureRequested(CSteamID instigator, byte x, byte y, uint instanceId,
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

            var @event = new UnturnedStructureTransformingEvent(
                new UnturnedStructureBuildable(data, drop), player, instigator, point,
                Quaternion.Euler(angleX * 2, angleY * 2, angleZ * 2))
            {
                IsCancelled = !shouldAllow
            };

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
            point = @event.Point;

            var eulerAngles = @event.Rotation.eulerAngles;

            angleX = MeasurementTool.angleToByte(Mathf.RoundToInt(eulerAngles.x / 2f) * 2);
            angleY = MeasurementTool.angleToByte(Mathf.RoundToInt(eulerAngles.y / 2f) * 2);
            angleZ = MeasurementTool.angleToByte(Mathf.RoundToInt(eulerAngles.z / 2f) * 2);
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
        private static event BarricadeDeployed OnBarricadeDeployed;

        private delegate void StructureDeployed(StructureData data, StructureDrop drop);
        private static event StructureDeployed OnStructureDeployed;

        private delegate void BarricadeDestroyed(BarricadeData data, BarricadeDrop drop);
        private static event BarricadeDestroyed OnBarricadeDestroyed;

        private delegate void StructureDestroyed(StructureData data, StructureDrop drop);
        private static event StructureDestroyed OnStructureDestroyed;

        private delegate void BarricadeTransformed(BarricadeData data, BarricadeDrop drop, CSteamID instigatorSteamId);
        private static event BarricadeTransformed OnBarricadeTransformed;

        private delegate void StructureTransformed(StructureData data, StructureDrop drop, CSteamID instigatorSteamId);
        private static event StructureTransformed OnStructureTransformed;

        private static CSteamID CurrentTransformingPlayerId = CSteamID.Nil;

        [HarmonyPatch]
        // ReSharper disable once UnusedType.Local
        private class Patches
        {
            [HarmonyPatch(typeof(BarricadeManager), "dropBarricadeIntoRegionInternal")]
            [HarmonyPostfix]
            private static void DropBarricade(BarricadeRegion region, BarricadeData data, ref Transform result,
                ref uint instanceID)
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

            [HarmonyPatch(typeof(StructureManager), nameof(StructureManager.dropReplicatedStructure))]
            [HarmonyPostfix]
            private static void DropStructure(Vector3 point, bool __result, uint ___instanceCount)
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

            [HarmonyPatch(typeof(BarricadeManager), nameof(BarricadeManager.destroyBarricade))]
            [HarmonyPrefix]
            private static void DestroyBarricade(BarricadeRegion region, ushort index)
            {
                ThreadUtil.assertIsGameThread();

                var data = region.barricades[index];
                var drop = region.drops[index];

                OnBarricadeDestroyed?.Invoke(data, drop);
            }

            [HarmonyPatch(typeof(StructureManager), nameof(StructureManager.destroyStructure))]
            [HarmonyPrefix]
            private static void DestroyStructure(StructureRegion region, ushort index)
            {
                ThreadUtil.assertIsGameThread();

                var data = region.structures[index];
                var drop = region.drops[index];

                OnStructureDestroyed?.Invoke(data, drop);
            }

            [HarmonyPatch(typeof(BarricadeManager), nameof(BarricadeManager.askTransformBarricade))]
            [HarmonyPrefix]
            private static void PreAskTransformBarricade(CSteamID steamID)
            {
                CurrentTransformingPlayerId = steamID;
            }

            [HarmonyPatch(typeof(BarricadeManager), nameof(BarricadeManager.askTransformBarricade))]
            [HarmonyPostfix]
            private static void PostAskTransformBarricade()
            {
                CurrentTransformingPlayerId = CSteamID.Nil;
            }

            [HarmonyPatch(typeof(StructureManager), nameof(StructureManager.askTransformStructure))]
            [HarmonyPrefix]
            private static void PreAskTransformStructure(CSteamID steamID)
            {
                CurrentTransformingPlayerId = steamID;
            }

            [HarmonyPatch(typeof(StructureManager), nameof(StructureManager.askTransformStructure))]
            [HarmonyPostfix]
            private static void PostAskTransformStructure()
            {
                CurrentTransformingPlayerId = CSteamID.Nil;
            }

            [HarmonyPatch(typeof(BarricadeManager), nameof(BarricadeManager.tellTransformBarricade))]
            [HarmonyPostfix]
            private static void TellTransformBarricade(BarricadeManager __instance, CSteamID steamID, byte x, byte y, ushort plant, uint instanceID)
            {
                ThreadUtil.assertIsGameThread();

                if (!__instance.channel.checkServer(steamID) ||
                    !BarricadeManager.tryGetRegion(x, y, plant, out var region)) return;

                var index = region.barricades.FindIndex(k => k.instanceID == instanceID);

                if (index < 0) return;

                var data = region.barricades[index];
                var drop = region.drops[index];

                OnBarricadeTransformed?.Invoke(data, drop, CurrentTransformingPlayerId);
            }

            [HarmonyPatch(typeof(StructureManager), nameof(StructureManager.tellTransformStructure))]
            [HarmonyPostfix]
            private static void TellTransformStructure(StructureManager __instance, CSteamID steamID, byte x, byte y, uint instanceID)
            {
                ThreadUtil.assertIsGameThread();

                if (!__instance.channel.checkServer(steamID) ||
                    !StructureManager.tryGetRegion(x, y, out var region)) return;

                var index = region.structures.FindIndex(k => k.instanceID == instanceID);

                if (index < 0) return;

                var data = region.structures[index];
                var drop = region.drops[index];

                OnStructureTransformed?.Invoke(data, drop, CurrentTransformingPlayerId);
            }
        }
    }
}
