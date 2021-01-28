namespace OpenMod.Extensions.Games.Abstractions.Vehicles
{
    /// <summary>
    /// Represents entities that can enter vehicles.
    /// </summary>
    public interface ICanEnterVehicle
    {
        /// <summary>
        /// Gets the current vehicle. Returns null if the entity is not a passenger.
        /// </summary>
        public IVehicle? CurrentVehicle { get; }
    }
}