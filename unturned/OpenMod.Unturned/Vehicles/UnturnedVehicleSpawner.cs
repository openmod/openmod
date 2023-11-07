using Cysharp.Threading.Tasks;
using OpenMod.API.Ioc;
using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Extensions.Games.Abstractions.Transforms;
using OpenMod.Extensions.Games.Abstractions.Vehicles;
using OpenMod.UnityEngine.Extensions;
using SDG.Unturned;
using Steamworks;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Priority = OpenMod.API.Prioritization.Priority;
using Quaternion = System.Numerics.Quaternion;
using UVector3 = UnityEngine.Vector3;
using Vector3 = System.Numerics.Vector3;

namespace OpenMod.Unturned.Vehicles
{
    [ServiceImplementation(Priority = Priority.Lowest)]
    public class UnturnedVehicleSpawner : IVehicleSpawner
    {
        public Task<IVehicle?> SpawnVehicleAsync(Vector3 position, Quaternion rotation, string vehicleAssetId, IVehicleState? state = null)
        {
            async UniTask<IVehicle?> SpawnVehicleTask()
            {
                if (!ushort.TryParse(vehicleAssetId, out var parsedVehicleId))
                {
                    throw new Exception($"Invalid vehicle id: {vehicleAssetId}");
                }

                if (Assets.find(EAssetType.VEHICLE, parsedVehicleId) is not VehicleAsset vehicleAsset)
                {
                    return null;
                }

                UnturnedVehicle? vehicle = null;
                if (state?.StateData?.Length > 0)
                {
                    ReadState(state.StateData, out var skinId, out var mythicId,
                        out var roadPosition, out var fuel, out var health, out var batteryCharge,
                        out var itemBatteryId, out var owner, out var group, out var locked, out byte[][] turrets,
                        out var tireAliveMask, out var items);

                    await UniTask.SwitchToMainThread();

                    var iVehicle = VehicleManager.SpawnVehicleV3(vehicleAsset, skinId, mythicId, roadPosition,
                        position.ToUnityVector(), rotation.ToUnityQuaternion(), false, false, false,
                        false, fuel, health, batteryCharge, owner, group, locked, turrets, tireAliveMask);

                    if (iVehicle == null)
                        return vehicle;

                    vehicle = new UnturnedVehicle(iVehicle);
                    if (itemBatteryId != Guid.Empty)
                    {
                        UnturnedVehicleState.BatteryItemGuidField?.SetValue(iVehicle, itemBatteryId);
                    }

                    if (items == null)
                        return vehicle;

                    foreach (var item in items)
                    {
                        iVehicle.trunkItems.loadItem(item.x, item.y, item.rot, item.item);
                    }
                }
                else
                {
                    await UniTask.SwitchToMainThread();

                    var iVehicle = VehicleManager.spawnVehicleV2(parsedVehicleId, position.ToUnityVector(),
                        Quaternion.Identity.ToUnityQuaternion());
                    if (iVehicle != null)
                    {
                        vehicle = new UnturnedVehicle(iVehicle);
                    }
                }

                return vehicle;
            }

            return SpawnVehicleTask().AsTask();
        }

        public Task<IVehicle?> SpawnVehicleAsync(IPlayer player, string vehicleAssetId, IVehicleState? state = null)
        {
            var rotation = player.Transform.Rotation;
            var position = player.Transform.Position;

            var forward = (Vector3.UnitZ * 6).Rotate(rotation);

            position += forward;
            
            Physics.Raycast((position + Vector3.UnitY * 16f).ToUnityVector(), UVector3.down, out var raycastHit, 32f, RayMasks.BLOCK_VEHICLE);
            if (raycastHit.collider != null)
            {
                position.Y = raycastHit.point.y + 16f;
            }

            return SpawnVehicleAsync(position, rotation, vehicleAssetId, state);
        }

        private void ReadState(byte[] buffer, out ushort skinId, out ushort mythicId,
            out float roadPosition, out ushort fuel, out ushort health, out ushort batteryCharge, out Guid itemBatteryId,
            out CSteamID owner, out CSteamID group, out bool locked, out byte[][] turrets, out byte tireAliveMask, out ItemJar[]? items)
        {
            using var stream = new MemoryStream(buffer, false);
            using var reader = new BinaryReader(stream);

            var version = UnturnedVehicleState.InitialSaveDataVersion;
            // if the last value of buffer is 255, then read the version
            if (buffer[^1] == 255)
            {
                version = reader.ReadByte();
            }

            if (version == UnturnedVehicleState.InitialSaveDataVersion)
            {
                reader.ReadUInt16(); // id
                reader.ReadUInt32(); // instanceId
            }

            skinId = reader.ReadUInt16();
            mythicId = reader.ReadUInt16();
            roadPosition = reader.ReadSingle();
            fuel = reader.ReadUInt16();
            health = reader.ReadUInt16();
            batteryCharge = reader.ReadUInt16();
            
            if (version >= UnturnedVehicleState.SaveDataVersionBatteryGuid)
            {
                var guidBuffer = new byte[16];
                _ = reader.Read(guidBuffer, 0, 16);
                itemBatteryId = new Guid(guidBuffer);
            }
            else
            {
                itemBatteryId = Guid.Empty;
            }

            tireAliveMask = reader.ReadByte();
            owner = new CSteamID(reader.ReadUInt64());
            group = new CSteamID(reader.ReadUInt64());
            locked = reader.ReadBoolean();

            var turretsLength = reader.ReadByte();
            turrets = new byte[turretsLength][];
            for (var b = 0; b < turretsLength; b++)
            {
                var stateLength = reader.ReadByte();
                turrets[b] = new byte[stateLength];
                _ = reader.Read(turrets[b], 0, stateLength);
            }

            var hasTruckItems = reader.ReadBoolean();
            if (!hasTruckItems)
            {
                items = null;
                return;
            }

            var count = reader.ReadByte();
            items = new ItemJar[count];
            for (var b = 0; b < count; b++)
            {
                var x = reader.ReadByte();
                var y = reader.ReadByte();
                var rot = reader.ReadByte();
                var id = reader.ReadUInt16();
                var amount = reader.ReadByte();
                var quality = reader.ReadByte();

                var stateLength = reader.ReadByte();
                var state = new byte[stateLength];

                _ = reader.Read(state, 0, stateLength);

                if (Assets.find(EAssetType.ITEM, id) is not ItemAsset)
                    continue;

                var item = new Item(id, amount, quality, state);
                items[b] = new ItemJar(x, y, rot, item);
            }
        }
    }
}