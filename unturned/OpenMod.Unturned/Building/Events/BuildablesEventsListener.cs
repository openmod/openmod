using HarmonyLib;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.Unturned.Events;
using OpenMod.Unturned.Players;
using SDG.Unturned;
using Steamworks;
using System.Linq;
using UnityEngine;

namespace OpenMod.Unturned.Building.Events
{
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

        private void OnDamageBarricadeRequested(CSteamID instigatorSteamID, Transform barricadeTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            if (BarricadeManager.tryGetInfo(barricadeTransform, out byte x, out byte y, out ushort plant,
                out ushort index, out BarricadeRegion region, out BarricadeDrop drop))
            {
                BarricadeData data = region.barricades[index];

                UnturnedBarricadeBuildable buildable = new UnturnedBarricadeBuildable(data, drop);

                Player nativePlayer = Provider.clients.FirstOrDefault(x => x?.playerID.steamID == instigatorSteamID)?.player;

                UnturnedPlayer player = nativePlayer == null ? null : new UnturnedPlayer(nativePlayer);

                UnturnedBuildableDamagingEvent @event;

                if (pendingTotalDamage >= buildable.State.Health)
                {
                    @event = new UnturnedBarricadeDestroyingEvent(buildable, pendingTotalDamage, damageOrigin, player, instigatorSteamID);
                }
                else
                {
                    @event = new UnturnedBarricadeDamagingEvent(buildable, pendingTotalDamage, damageOrigin, player, instigatorSteamID);
                }

                Emit(@event);

                pendingTotalDamage = @event.DamageAmount;
                shouldAllow = !@event.IsCancelled;
            }
        }

        private void OnDamageStructureRequested(CSteamID instigatorSteamID, Transform structureTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            if (StructureManager.tryGetInfo(structureTransform, out byte x, out byte y, out ushort index,
                out StructureRegion region, out StructureDrop drop))
            {
                StructureData data = region.structures[index];

                UnturnedStructureBuildable buildable = new UnturnedStructureBuildable(data, drop);

                Player nativePlayer = Provider.clients.FirstOrDefault(x => x?.playerID.steamID == instigatorSteamID)?.player;

                UnturnedPlayer player = nativePlayer == null ? null : new UnturnedPlayer(nativePlayer);

                UnturnedBuildableDamagingEvent @event;

                if (pendingTotalDamage >= buildable.State.Health)
                {
                    @event = new UnturnedStructureDestroyingEvent(buildable, pendingTotalDamage, damageOrigin, player, instigatorSteamID);
                }
                else
                {
                    @event = new UnturnedStructureDamagingEvent(buildable, pendingTotalDamage, damageOrigin, player, instigatorSteamID);
                }

                Emit(@event);

                pendingTotalDamage = @event.DamageAmount;
                shouldAllow = !@event.IsCancelled;
            }
        }

        private void Events_OnBarricadeDeployed(BarricadeData data, BarricadeDrop drop)
        {
            UnturnedBarricadeDeployedEvent @event = new UnturnedBarricadeDeployedEvent(new UnturnedBarricadeBuildable(data, drop));

            Emit(@event);
        }

        private void Events_OnStructureDeployed(StructureData data, StructureDrop drop)
        {
            UnturnedStructureDeployedEvent @event = new UnturnedStructureDeployedEvent(new UnturnedStructureBuildable(data, drop));

            Emit(@event);
        }

        private void OnSalvageBarricadeRequested(CSteamID steamID, byte x, byte y, ushort plant, ushort index, ref bool shouldAllow)
        {
            BarricadeData data = BarricadeManager.regions[x, y].barricades[index];
            BarricadeDrop drop = BarricadeManager.regions[x, y].drops[index];

            UnturnedBarricadeSalvagingEvent @event =
                new UnturnedBarricadeSalvagingEvent(new UnturnedBarricadeBuildable(data, drop));

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private void OnSalvageStructureRequested(CSteamID steamID, byte x, byte y, ushort index, ref bool shouldAllow)
        {
            StructureData data = StructureManager.regions[x, y].structures[index];
            StructureDrop drop = StructureManager.regions[x, y].drops[index];

            UnturnedStructureSalvagingEvent @event =
                new UnturnedStructureSalvagingEvent(new UnturnedStructureBuildable(data, drop));

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
        }

        private void Events_OnBarricadeDestroyed(BarricadeData data, BarricadeDrop drop)
        {
            UnturnedBarricadeDestroyedEvent @event =
                new UnturnedBarricadeDestroyedEvent(new UnturnedBarricadeBuildable(data, drop));

            Emit(@event);
        }

        private void Events_OnStructureDestroyed(StructureData data, StructureDrop drop)
        {
            UnturnedStructureDestroyedEvent @event =
                new UnturnedStructureDestroyedEvent(new UnturnedStructureBuildable(data, drop));

            Emit(@event);
        }

        private void OnTransformBarricadeRequested(CSteamID instigator, byte x, byte y, ushort plant, uint instanceID,
            ref Vector3 point, ref byte angle_x, ref byte angle_y, ref byte angle_z, ref bool shouldAllow)
        {
            BarricadeRegion region = BarricadeManager.regions[x, y];
            int index = region.barricades.FindIndex(k => k.instanceID == instanceID);

            BarricadeData data = region.barricades[index];
            BarricadeDrop drop = region.drops[index];

            Player nativePlayer = Provider.clients.FirstOrDefault(x => x?.playerID.steamID == instigator)?.player;

            UnturnedPlayer player = nativePlayer == null ? null : new UnturnedPlayer(nativePlayer);

            UnturnedBarricadeTransformingEvent @event = new UnturnedBarricadeTransformingEvent(
                new UnturnedBarricadeBuildable(data, drop), player, instigator, point,
                Quaternion.Euler(angle_x * 2, angle_y * 2, angle_z * 2));

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
            point = @event.Point;

            Vector3 eulerAngles = @event.Rotation.eulerAngles;

            angle_x = MeasurementTool.angleToByte(Mathf.RoundToInt(eulerAngles.x / 2f) * 2);
            angle_y = MeasurementTool.angleToByte(Mathf.RoundToInt(eulerAngles.y / 2f) * 2);
            angle_z = MeasurementTool.angleToByte(Mathf.RoundToInt(eulerAngles.z / 2f) * 2);
        }

        private void OnTransformStructureRequested(CSteamID instigator, byte x, byte y, uint instanceID,
            ref Vector3 point, ref byte angle_x, ref byte angle_y, ref byte angle_z, ref bool shouldAllow)
        {
            StructureRegion region = StructureManager.regions[x, y];
            int index = region.structures.FindIndex(k => k.instanceID == instanceID);

            StructureData data = region.structures[index];
            StructureDrop drop = region.drops[index];

            Player nativePlayer = Provider.clients.FirstOrDefault(x => x?.playerID.steamID == instigator)?.player;

            UnturnedPlayer player = nativePlayer == null ? null : new UnturnedPlayer(nativePlayer);

            UnturnedStructureTransformingEvent @event = new UnturnedStructureTransformingEvent(
                new UnturnedStructureBuildable(data, drop), player, instigator, point,
                Quaternion.Euler(angle_x * 2, angle_y * 2, angle_z * 2));

            Emit(@event);

            shouldAllow = !@event.IsCancelled;
            point = @event.Point;

            Vector3 eulerAngles = @event.Rotation.eulerAngles;

            angle_x = MeasurementTool.angleToByte(Mathf.RoundToInt(eulerAngles.x / 2f) * 2);
            angle_y = MeasurementTool.angleToByte(Mathf.RoundToInt(eulerAngles.y / 2f) * 2);
            angle_z = MeasurementTool.angleToByte(Mathf.RoundToInt(eulerAngles.z / 2f) * 2);
        }

        private void Events_OnBarricadeTransformed(BarricadeData data, BarricadeDrop drop)
        {
            UnturnedBarricadeTransformedEvent @event =
                new UnturnedBarricadeTransformedEvent(new UnturnedBarricadeBuildable(data, drop));

            Emit(@event);
        }

        private void Events_OnStructureTransformed(StructureData data, StructureDrop drop)
        {
            UnturnedStructureTransformedEvent @event =
                new UnturnedStructureTransformedEvent(new UnturnedStructureBuildable(data, drop));

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

        private delegate void BarricadeTransformed(BarricadeData data, BarricadeDrop drop);
        private static event BarricadeTransformed OnBarricadeTransformed;

        private delegate void StructureTransformed(StructureData data, StructureDrop drop);
        private static event StructureTransformed OnStructureTransformed;

        [HarmonyPatch]
        private class Patches
        {
            [HarmonyPatch(typeof(BarricadeManager), "dropBarricadeIntoRegionInternal")]
            [HarmonyPostfix]
            private static void DropBarricade(BarricadeRegion region, Barricade barricade, Vector3 point,
                Quaternion rotation, ulong owner, ulong group, BarricadeData data, ref Transform result,
                ref uint instanceID)
            {
                if (result != null)
                {
                    BarricadeDrop drop = region.drops.LastOrDefault();

                    if (drop?.instanceID == instanceID)
                    {
                        OnBarricadeDeployed?.Invoke(data, drop);
                    }
                }
            }

            [HarmonyPatch(typeof(StructureManager), "dropReplicatedStructure")]
            [HarmonyPostfix]
            private static void DropStructure(Structure structure, Vector3 point, Quaternion rotation, ulong owner,
                ulong group, bool __result, uint ___instanceCount)
            {
                if (__result)
                {
                    if (Regions.tryGetCoordinate(point, out byte b, out byte b2))
                    {
                        if (StructureManager.tryGetRegion(b, b2, out StructureRegion region))
                        {
                            StructureData data = region.structures.LastOrDefault();
                            StructureDrop drop = region.drops.LastOrDefault();

                            if (data?.instanceID == ___instanceCount && drop?.instanceID == ___instanceCount)
                            {
                                OnStructureDeployed?.Invoke(data, drop);
                            }
                        }
                    }
                }
            }

            [HarmonyPatch(typeof(BarricadeManager), "destroyBarricade")]
            [HarmonyPrefix]
            private static void DestroyBarricade(BarricadeRegion region, ushort index)
            {
                ThreadUtil.assertIsGameThread();

                BarricadeData data = region.barricades[index];
                BarricadeDrop drop = region.drops[index];

                OnBarricadeDestroyed?.Invoke(data, drop);
            }

            [HarmonyPatch(typeof(StructureManager), "destroyStructure")]
            [HarmonyPrefix]
            private static void DestroyStructure(StructureRegion region, ushort index)
            {
                ThreadUtil.assertIsGameThread();

                StructureData data = region.structures[index];
                StructureDrop drop = region.drops[index];

                OnStructureDestroyed?.Invoke(data, drop);
            }


            [HarmonyPatch(typeof(BarricadeManager), "askTransformBarricade")]
            [HarmonyPostfix]
            public static void AskTransformBarricade(byte x, byte y, ushort plant, uint instanceID)
            {
                ThreadUtil.assertIsGameThread();

                if (!BarricadeManager.tryGetRegion(x, y, plant, out BarricadeRegion region)) return;


                int index = region.barricades.FindIndex(k => k.instanceID == instanceID);
                BarricadeData data = region.barricades[index];
                BarricadeDrop drop = region.drops[index];

                OnBarricadeTransformed?.Invoke(data, drop);
            }

            [HarmonyPatch(typeof(StructureManager), "askTransformStructure")]
            [HarmonyPostfix]
            public static void AskTransformStructure(byte x, byte y, uint instanceID)
            {
                ThreadUtil.assertIsGameThread();

                if (!StructureManager.tryGetRegion(x, y, out StructureRegion region)) return;

                int index = region.structures.FindIndex(k => k.instanceID == instanceID);
                StructureData data = region.structures[index];
                StructureDrop drop = region.drops[index];

                OnStructureTransformed?.Invoke(data, drop);
            }
        }
    }
}
