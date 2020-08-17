using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using OpenMod.Extensions.Games.Abstractions.Entities;
using OpenMod.Extensions.Games.Abstractions.Transforms;

namespace OpenMod.Extensions.Games.Abstractions.Vehicles
{
    public interface IVehicleState
    {
        [CanBeNull]
        byte[] StateData { get; }
    }

    public interface IVehicle
    {
        IVehicleAsset Asset { get; }

        IVehicleState State { get; }

        IWorldTransform Transform { get; }

        string VehicleInstanceId { get; }

        [CanBeNull]
        IEntity Driver { get; }

        IReadOnlyCollection<IEntity> Passengers { get; }

        Task<bool> AddPassengerAsync(IEntity passenger);
        
        Task<bool> RemovePassengerAsync(IEntity passenger);

        Task DestroyAsync();
    }
}