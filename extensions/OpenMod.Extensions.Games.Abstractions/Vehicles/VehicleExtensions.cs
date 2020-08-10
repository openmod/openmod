using System.Numerics;
using System.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Players;

namespace OpenMod.Extensions.Games.Abstractions.Vehicles
{
    public static class VehicleExtensions
    {
        public static async Task SpawnVehicleAsync(this IVehicleSpawner vehicleSpawner, IPlayer player, string vehicleAssetId)
        {
            // todo: spawn in vehicle in front of player

            // very stupid random offset as a workaround for now
            var offset = new Vector3(5, 2, 5);
            await vehicleSpawner.SpawnVehicleAsync(player.Position + offset, vehicleAssetId);
        }
    }
}