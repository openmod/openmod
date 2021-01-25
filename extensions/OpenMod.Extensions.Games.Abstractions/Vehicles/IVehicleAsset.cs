namespace OpenMod.Extensions.Games.Abstractions.Vehicles
{
    /// <summary>
    /// Represents a vehicle asset.
    /// </summary>
    public interface IVehicleAsset
    {
        /// <value>
        /// The ID of the vehicle asset.
        /// </value>
        string VehicleAssetId { get; }

        /// <value>
        /// The type of the vehicle asset.
        /// </value>
        string VehicleType { get; }
    }
}