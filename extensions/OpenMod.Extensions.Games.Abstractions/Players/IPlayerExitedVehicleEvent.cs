using OpenMod.Extensions.Games.Abstractions.Vehicles;

namespace OpenMod.Extensions.Games.Abstractions.Players
{
    /// <summary>
    /// The event that is triggered when a player has exited a vehicle.
    /// </summary>
    public interface IPlayerExitedVehicleEvent : IPlayerEvent, IVehicleEvent
    {
    }
}