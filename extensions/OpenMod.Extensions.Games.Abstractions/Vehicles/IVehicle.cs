using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using OpenMod.Extensions.Games.Abstractions.Entities;

namespace OpenMod.Extensions.Games.Abstractions.Vehicles
{
    public interface IVehicleState
    {
        [CanBeNull]
        byte[] StateData { get; }
    }

    public interface IVehicle : IEntity
    {
        IVehicleAsset Asset { get; }

        IVehicleState State { get; }

        string VehicleInstanceId { get; }

        [CanBeNull]
        IEntity Driver { get; }

        IReadOnlyCollection<IEntity> Passengers { get; }

        Task SetVelocity(Vector3 velocity);

        Task RemovePassenger(IEntity passenger);

        Task DestroyAsync();
    }
}