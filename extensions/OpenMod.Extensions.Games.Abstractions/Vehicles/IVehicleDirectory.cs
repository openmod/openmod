using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.API.Ioc;

namespace OpenMod.Extensions.Games.Abstractions.Vehicles
{
    [Service]
    public interface IVehicleDirectory
    {
        Task<IReadOnlyCollection<IVehicleAsset>> GetVehicleAssetsAsync();

        Task<IReadOnlyCollection<IVehicle>> GetVehiclesAsync();
    }
}