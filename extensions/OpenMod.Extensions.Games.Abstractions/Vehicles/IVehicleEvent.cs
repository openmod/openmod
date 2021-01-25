using OpenMod.API.Eventing;

namespace OpenMod.Extensions.Games.Abstractions.Vehicles
{
    /// <summary>
    /// The base interface for all vehicle related events.
    /// </summary>
    public interface IVehicleEvent : IEvent
    {
        /// <value>
        /// The vehicle related to the event.
        /// </value>
        IVehicle Vehicle { get; }
    }
}