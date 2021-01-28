using OpenMod.API.Eventing;

namespace OpenMod.Extensions.Games.Abstractions.Vehicles
{
    /// <summary>
    /// The base interface for all vehicle related events.
    /// </summary>
    public interface IVehicleEvent : IEvent
    {
        /// <summary>
        /// Gets the vehicle related to the event.
        /// </summary>
        IVehicle Vehicle { get; }
    }
}