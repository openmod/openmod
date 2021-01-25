using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.API.Ioc;

namespace OpenMod.Extensions.Games.Abstractions.Vehicles
{
    /// <summary>
    /// The service for querying vehicles and vehicle assets.
    /// </summary>
    [Service]
    public interface IVehicleDirectory
    {
        /// <summary>
        /// Gets all vehicle assets.
        /// </summary>
        Task<IReadOnlyCollection<IVehicleAsset>> GetVehicleAssetsAsync();

        /// <summary>
        /// Gets all vehicles.
        /// </summary>
        Task<IReadOnlyCollection<IVehicle>> GetVehiclesAsync();
    }
}