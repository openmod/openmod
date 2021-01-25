using OpenMod.API.Eventing;
using OpenMod.Extensions.Games.Abstractions.Vehicles;

namespace OpenMod.Extensions.Games.Abstractions.Players
{
    /// <summary>
    /// The event that is triggered when a player is exiting a vehicle.
    /// </summary>
    public interface IPlayerExitingVehicleEvent : IPlayerEvent, IVehicleEvent, ICancellableEvent
    {
    }
}