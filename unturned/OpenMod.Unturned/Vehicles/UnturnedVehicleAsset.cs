using OpenMod.Extensions.Games.Abstractions.Vehicles;
using SDG.Unturned;
using System.Globalization;

namespace OpenMod.Unturned.Vehicles
{
    public class UnturnedVehicleAsset : IVehicleAsset
    {
        public VehicleAsset VehicleAsset { get; }

        public UnturnedVehicleAsset(VehicleAsset vehicleAsset)
        {
            VehicleAsset = vehicleAsset;
            VehicleAssetId = vehicleAsset.id.ToString();
            VehicleName = vehicleAsset.vehicleName;
            VehicleType = vehicleAsset.engine.ToString().ToLower(CultureInfo.InvariantCulture);
        }

        public string VehicleAssetId { get; }

        public string VehicleName { get; }

        public string VehicleType { get; }
    }
}