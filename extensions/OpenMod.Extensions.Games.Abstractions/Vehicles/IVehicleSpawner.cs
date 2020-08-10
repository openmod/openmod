using System.Numerics;
using System.Threading.Tasks;
using OpenMod.API.Ioc;

namespace OpenMod.Extensions.Games.Abstractions.Vehicles
{
    [Service]
    public interface IVehicleSpawner
    {
        Task SpawnVehicleAsync(Vector3 position, string vehicleAssetId);
    }
}