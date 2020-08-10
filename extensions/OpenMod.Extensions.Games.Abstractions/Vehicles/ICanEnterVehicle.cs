using JetBrains.Annotations;

namespace OpenMod.Extensions.Games.Abstractions.Vehicles
{
    public interface ICanEnterVehicle
    {
        [CanBeNull]
        public IVehicle CurrentVehicle { get; }
    }
}