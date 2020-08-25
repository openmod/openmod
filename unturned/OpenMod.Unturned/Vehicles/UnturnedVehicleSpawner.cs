using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using OpenMod.Extensions.Games.Abstractions.Vehicles;
using OpenMod.UnityEngine.Extensions;
using SDG.Unturned;
using Quaternion = UnityEngine.Quaternion;

namespace OpenMod.Unturned.Vehicles
{
    [ServiceImplementation(Priority = Priority.Lowest)]
    public class UnturnedVehicleSpawner : IVehicleSpawner
    {
        public Task<IVehicle> SpawnVehicleAsync(Vector3 position, string vehicleId, IVehicleState state = null)
        {
            async UniTask<IVehicle> VehicleSpawnTask()
            {
                await UniTask.SwitchToMainThread();
                if (!ushort.TryParse(vehicleId, out var parsedVehicleId))
                {
                    throw new Exception($"Invalid vehicle id: {vehicleId}");
                }

                var vehicleAsset = (ItemAsset)Assets.find(EAssetType.ITEM, parsedVehicleId);
                if (vehicleAsset == null || vehicleAsset.isPro)
                {
                    return null;
                }

                if (state != null)
                {
                    // todo: use state like VehicleManager.load() does
                }
                
                var vehicle = VehicleManager.spawnVehicleV2(vehicleAsset.id, position.ToUnityVector(), Quaternion.identity);
                return new UnturnedVehicle(vehicle);
            }

            return VehicleSpawnTask().AsTask();
        }
    }
}