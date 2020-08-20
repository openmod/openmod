using JetBrains.Annotations;

namespace OpenMod.Extensions.Games.Abstractions.Vehicles
{
    public interface IVehicleState
    {
        [CanBeNull]
        byte[] StateData { get; }
    }
}