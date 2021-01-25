using JetBrains.Annotations;

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
        [CanBeNull]
        public IVehicle CurrentVehicle { get; }
    }
}