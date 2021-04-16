using OpenMod.Extensions.Games.Abstractions.Players;
using System;
using System.Numerics;
using System.Threading.Tasks;

namespace OpenMod.Extensions.Games.Abstractions.Vehicles
{
    public static class VehicleSpawnerExtensions
    {
        /// See <see cref="IVehicleSpawner.SpawnVehicleAsync(IPlayer, string, IVehicleState?)"/>.
        [Obsolete("Use IVehicleSpawner.SpawnVehicleAsync(IPlayer, string, IVehicleState?) instead")]
        public static Task<IVehicle?> SpawnVehicleAsync(this IVehicleSpawner vehicleSpawner, IPlayer player, string vehicleAssetId)
        {
            return vehicleSpawner.SpawnVehicleAsync(player, vehicleAssetId, state: null);
        }

        /// <summary>
        /// Spawns a vehicle at the given position.
        /// </summary>
        /// <param name="vehicleSpawner">The vehicle spawner.</param>
        /// <param name="position">The position to spawn the vehicle at.</param>
        /// <param name="vehicleAssetId">The ID of the vehicle asset.</param>
        /// <param name="state">The optional state of the vehicle.</param>
        /// <returns><b>The spawned vehicle</b> if successful; otherwise, <b>null</b>.</returns>
        public static Task<IVehicle?> SpawnVehicleAsync(this IVehicleSpawner vehicleSpawner, Vector3 position, string vehicleAssetId, IVehicleState? state = null)
        {
            return vehicleSpawner.SpawnVehicleAsync(position, Quaternion.Identity, vehicleAssetId, state);
        }
    }
}