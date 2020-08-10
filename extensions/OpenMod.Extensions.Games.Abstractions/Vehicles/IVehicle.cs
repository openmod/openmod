using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using OpenMod.Extensions.Games.Abstractions.Entities;

namespace OpenMod.Extensions.Games.Abstractions.Vehicles
{
    public interface IVehicle : IEntity
    {
        string VehicleAssetId { get; }

        string VehicleInstanceId { get; }

        string VehicleType { get; }

        double Velocity { get; }

        [CanBeNull]
        IEntity Driver { get; }
        
        IReadOnlyCollection<IEntity> Passengers { get; }

        Task RemovePassenger(IEntity passenger); 

        Task DestroyAsync();
    }
}