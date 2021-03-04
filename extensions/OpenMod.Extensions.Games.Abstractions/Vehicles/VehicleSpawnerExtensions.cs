using OpenMod.Extensions.Games.Abstractions.Players;
using System.Numerics;
using System.Threading.Tasks;

namespace OpenMod.Extensions.Games.Abstractions.Vehicles
{
    public static class VehicleSpawnerExtensions
    {
        /// See <see cref="IVehicleSpawner.SpawnVehicleAsync"/>.
        public static async Task<IVehicle?> SpawnVehicleAsync(this IVehicleSpawner vehicleSpawner, IPlayer player, string vehicleAssetId)
        {
            // todo: spawn in vehicle in front of player

            // very stupid random offset as a workaround for now
            var offset = new Vector3(5, 2, 5);
            return await vehicleSpawner.SpawnVehicleAsync(player.Transform.Position + offset, vehicleAssetId);
        }
    }
}