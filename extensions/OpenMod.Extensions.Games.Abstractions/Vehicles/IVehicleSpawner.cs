using OpenMod.API.Ioc;
using OpenMod.Extensions.Games.Abstractions.Players;
using System.Numerics;
using System.Threading.Tasks;

namespace OpenMod.Extensions.Games.Abstractions.Vehicles
{
    /// <summary>
    /// The service for spawning vehicles.
    /// </summary>
    [Service]
    public interface IVehicleSpawner
    {
        /// <summary>
        /// Spawns a vehicle at the given position.
        /// </summary>
        /// <param name="position">The position to spawn the vehicle at.</param>
        /// <param name="rotation">The rotation to spawn the vehicle in.</param>
        /// <param name="vehicleAssetId">The ID of the vehicle asset.</param>
        /// <param name="state">The optional state of the vehicle.</param>
        /// <returns><b>The spawned vehicle</b> if successful; otherwise, <b>null</b>.</returns>
        Task<IVehicle?> SpawnVehicleAsync(Vector3 position, Quaternion rotation, string vehicleAssetId, IVehicleState? state = null);

        /// <summary>
        /// Spawns a vehicle for the given player.
        /// </summary>
        /// <param name="player">The player to spawn the vehicle for.</param>
        /// <param name="vehicleAssetId">The ID of the vehicle asset.</param>
        /// <param name="state">The optional state of the vehicle.</param>
        /// <returns><b>The spawned vehicle</b> if successful; otherwise, <b>null</b>.</returns>
        Task<IVehicle?> SpawnVehicleAsync(IPlayer player, string vehicleAssetId, IVehicleState? state = null);
    }
}