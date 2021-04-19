using Cysharp.Threading.Tasks;
using OpenMod.API.Ioc;
using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Extensions.Games.Abstractions.Transforms;
using OpenMod.Extensions.Games.Abstractions.Vehicles;
using OpenMod.UnityEngine.Extensions;
using SDG.Unturned;
using Steamworks;
using System;
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
                await UniTask.SwitchToMainThread();
                if (!ushort.TryParse(vehicleAssetId, out var parsedVehicleId))
                {
                    throw new Exception($"Invalid vehicle id: {vehicleAssetId}");
                }

                if (Assets.find(EAssetType.VEHICLE, parsedVehicleId) is not VehicleAsset vehicleAsset)
                {
                    return null;
                }

                UnturnedVehicle? vehicle = null;
                if (state is UnturnedVehicleState && state.StateData?.Length > 0)
                {
                    ReadState(state.StateData, out _ /* id doesn't require i guess? */, out var skinId, out var mythicId,
                        out var roadPosition, out var fuel, out var health, out var batteryCharge,
                        out var owner, out var group, out var locked, out byte[][] turrets,
                        out _, out var tireAliveMask, out var items);

                    var iVehicle = VehicleManager.SpawnVehicleV3(vehicleAsset, skinId, mythicId, roadPosition,
                        position.ToUnityVector(), rotation.ToUnityQuaternion(), false, false, false,
                        false, fuel, health, batteryCharge, owner, group, locked, turrets, tireAliveMask);

                    if (iVehicle != null)
                    {
                        vehicle = new UnturnedVehicle(iVehicle);

                        if (items != null)
                        {
                            foreach (var item in items)
                            {
                                iVehicle.trunkItems.loadItem(item.x, item.y, item.rot, item.item);
                            }
                        }
                    }
                }
                else
                {
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

        private void ReadState(byte[] buffer, out ushort id, out ushort skinID, out ushort mythicID,
            out float roadPosition, out ushort fuel, out ushort health, out ushort batteryCharge, out CSteamID owner,
            out CSteamID group, out bool locked, out byte[][] turrets, out uint instanceID, out byte tireAliveMask,
            out ItemJar[]? items)

        {
            var step = 0;

            id = BitConverter.ToUInt16(buffer, step);
            step += 2;
            instanceID = BitConverter.ToUInt32(buffer, step);
            step += 4;
            skinID = BitConverter.ToUInt16(buffer, step);
            step += 2;
            mythicID = BitConverter.ToUInt16(buffer, step);
            step += 2;
            roadPosition = BitConverter.ToSingle(buffer, step);
            step += 4;
            fuel = BitConverter.ToUInt16(buffer, step);
            step += 2;
            health = BitConverter.ToUInt16(buffer, step);
            step += 2;
            batteryCharge = BitConverter.ToUInt16(buffer, step);
            step += 2;
            tireAliveMask = buffer[step];
            step++;
            owner = (CSteamID)BitConverter.ToUInt64(buffer, step);
            step += 8;
            group = (CSteamID)BitConverter.ToUInt16(buffer, step);
            step += 8;
            locked = BitConverter.ToBoolean(buffer, step);
            step++;

            var turretsLength = buffer[step];
            step++;
            turrets = new byte[turretsLength][];
            for (var b = 0; b < turretsLength; b++)
            {
                var stateLength = buffer[step];
                step++;
                turrets[b] = new byte[stateLength];
                Buffer.BlockCopy(buffer, step, turrets[b], 0, stateLength);
                step += stateLength;
            }

            var hasTruckItems = BitConverter.ToBoolean(buffer, step);
            step++;
            items = null;
            if (hasTruckItems)
            {
                var count = buffer[step];
                step++;
                items = new ItemJar[count];
                for (var b = 0; b < count; b++)
                {
                    var x = buffer[step];
                    step++;
                    var y = buffer[step];
                    step++;
                    var rot = buffer[step];
                    step++;
                    var itemId = BitConverter.ToUInt16(buffer, step);
                    step += 2;
                    var amount = buffer[step];
                    step++;
                    var quality = buffer[step];
                    step++;
                    var stateLength = buffer[step];
                    step++;
                    var itemState = new byte[stateLength];
                    Buffer.BlockCopy(buffer, step, itemState, 0, stateLength);
                    step += stateLength;

                    if (Assets.find(EAssetType.ITEM, itemId) is ItemAsset)
                    {
                        var item = new Item(itemId, amount, quality, itemState);
                        items[b] = new ItemJar(x, y, rot, item);
                    }
                }
            }
        }
    }
}