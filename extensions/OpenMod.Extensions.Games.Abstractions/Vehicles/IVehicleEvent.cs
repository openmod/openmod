using OpenMod.API.Eventing;

namespace OpenMod.Extensions.Games.Abstractions.Vehicles
{
    public interface IVehicleEvent : IEvent
    {
        IVehicle Vehicle { get; }
    }
}