using MoreLinq;
using OpenMod.Core.Helpers;
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

        /// <summary>
        /// Searches for vehicles by the vehicle asset name.
        /// </summary>
        /// <param name="directory">The item directory service.</param>
        /// <param name="vehicleName">The name of the vehicle asset.</param>
        /// <param name="exact">If true, only exact name matches will be used.</param>
        /// <returns><b>The <see cref="IVehicleAsset"/></b> if found; otherwise, <b>null</b>.</returns>
        public static async Task<IVehicleAsset?> FindByNameAsync(this IVehicleDirectory directory, string vehicleName, bool exact = true)
        {
            if (exact)
                return (await directory.GetVehicleAssetsAsync()).FirstOrDefault(d =>
                    d.VehicleName.Equals(vehicleName, StringComparison.OrdinalIgnoreCase));

            return (await directory.GetVehicleAssetsAsync())
                .Where(x => x.VehicleName.IndexOf(vehicleName, StringComparison.OrdinalIgnoreCase) >= 0)
                .MinBy(asset => StringHelper.LevenshteinDistance(vehicleName, asset.VehicleName))
                .FirstOrDefault();
        }
    }
}
