using System.Numerics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using OpenMod.API.Ioc;

namespace OpenMod.Extensions.Games.Abstractions.Vehicles
{
    [Service]
    public interface IVehicleSpawner
    {
        [CanBeNull]
        Task<IVehicle> SpawnVehicleAsync(Vector3 position, string vehicleId, [CanBeNull] IVehicleState state = null);
    }
}