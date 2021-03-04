using System;
using System.Linq;
using System.Threading.Tasks;

namespace OpenMod.Extensions.Games.Abstractions.Vehicles
{
    public static class VehicleDirectoryExtensions
    {
        /// <summary>
        /// Searches for vehicles by the vehicle asset id.
        /// </summary>
        /// <param name="directory">The vehicle directory service.</param>
        /// <param name="vehicleAssetId">The vehicle asset id to search for.</param>
        /// <returns><b>The <see cref="IVehicleAsset"/></b> if found; otherwise, <b>null</b>.</returns>
        public static async Task<IVehicleAsset?> FindByIdAsync(this IVehicleDirectory directory, string vehicleAssetId)
        {
            return (await directory.GetVehicleAssetsAsync())
                .FirstOrDefault(d => d.VehicleAssetId.Equals(vehicleAssetId, StringComparison.OrdinalIgnoreCase));
        }
    }
}
