using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using OpenMod.Extensions.Games.Abstractions.Acl;
using OpenMod.Extensions.Games.Abstractions.Entities;
using OpenMod.Extensions.Games.Abstractions.Transforms;

namespace OpenMod.Extensions.Games.Abstractions.Vehicles
{
    public interface IVehicle : IHasOwnership
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