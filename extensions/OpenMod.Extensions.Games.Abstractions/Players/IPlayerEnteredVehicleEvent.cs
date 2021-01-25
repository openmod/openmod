using OpenMod.Extensions.Games.Abstractions.Vehicles;

namespace OpenMod.Extensions.Games.Abstractions.Players
{
    /// <summary>
    /// The event that is triggered when a player has entered a vehicle.
    /// </summary>
    public interface IPlayerEnteredVehicleEvent : IPlayerEvent, IVehicleEvent
    {
    }
}