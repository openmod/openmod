namespace OpenMod.Extensions.Games.Abstractions.Vehicles
{
    /// <value>
    /// Represents entities that can enter vehicles.
    /// </value>
    public interface ICanEnterVehicle
    {
        /// <value>
        /// The current vehicle. Can be null.
        /// </value>
        public IVehicle? CurrentVehicle { get; }
    }
}