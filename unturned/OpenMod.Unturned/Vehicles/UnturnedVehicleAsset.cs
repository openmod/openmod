using System.Globalization;
using OpenMod.Extensions.Games.Abstractions.Vehicles;
using SDG.Unturned;

namespace OpenMod.Unturned.Vehicles
{
    public class UnturnedVehicleAsset : IVehicleAsset
    {
        public VehicleAsset VehicleAsset { get; }

        public UnturnedVehicleAsset(VehicleAsset vehicleAsset)
        {
            VehicleAsset = vehicleAsset;
            VehicleAssetId = vehicleAsset.id.ToString();
            VehicleType = vehicleAsset.engine.ToString().ToLower(CultureInfo.InvariantCulture);
        }

        public string VehicleAssetId { get; }

        public string VehicleType { get; }
    }
}