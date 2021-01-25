using OpenMod.API.Eventing;
using OpenMod.Extensions.Games.Abstractions.Vehicles;

namespace OpenMod.Extensions.Games.Abstractions.Players
{
    /// <summary>
    /// The event that is triggered when a player is entering a vehicle.
    /// </summary>
    public interface IPlayerEnteringVehicleEvent : IPlayerEvent, IVehicleEvent, ICancellableEvent
    {
    }
}