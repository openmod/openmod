using JetBrains.Annotations;

namespace OpenMod.Extensions.Games.Abstractions.Vehicles
{
    /// <summary>
    /// Represents the state of a vehicle.
    /// </summary>
    public interface IVehicleState
    {
        /// <summary>
        /// The vehicle state. Can be null.
        /// </summary>
        [CanBeNull]
        byte[] StateData { get; }
    }
}