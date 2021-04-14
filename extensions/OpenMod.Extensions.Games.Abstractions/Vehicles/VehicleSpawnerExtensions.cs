using OpenMod.Extensions.Games.Abstractions.Players;
using System;
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
    }
}