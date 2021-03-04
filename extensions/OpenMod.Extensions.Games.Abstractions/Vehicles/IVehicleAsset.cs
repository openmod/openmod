namespace OpenMod.Extensions.Games.Abstractions.Vehicles
{
    /// <summary>
    /// Represents a vehicle asset.
    /// </summary>
    public interface IVehicleAsset
    {
        /// <summary>
        /// Gets the ID of the vehicle asset.
        /// </summary>
        string VehicleAssetId { get; }

        /// <summary>
        /// The human readable name of the asset.
        /// </summary>
        string VehicleName { get; }

        /// <summary>
        /// Gets the type of the vehicle asset.
        /// </summary>
        string VehicleType { get; }
    }
}