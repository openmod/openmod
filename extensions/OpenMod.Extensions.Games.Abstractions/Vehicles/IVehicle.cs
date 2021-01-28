using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Acl;
using OpenMod.Extensions.Games.Abstractions.Entities;
using OpenMod.Extensions.Games.Abstractions.Transforms;

namespace OpenMod.Extensions.Games.Abstractions.Vehicles
{
    /// <summary>
    /// Represents a vehicle.
    /// </summary>
    public interface IVehicle : IHasOwnership
    {
        /// <summary>
        /// Gets the asset of the vehicle.
        /// </summary>
        IVehicleAsset Asset { get; }

        /// <summary>
        /// Gets the state of the vehicle.
        /// </summary>
        IVehicleState State { get; }

        /// <summary>
        /// Gets the transform of the vehicle.
        /// </summary>
        IWorldTransform Transform { get; }

        /// <summary>
        /// Gets the unique instance ID of the vehicle.
        /// </summary>
        string VehicleInstanceId { get; }

        /// <summary>
        /// Gets the driver of the vehicle.
        /// </summary>
        IEntity? Driver { get; }

        /// <summary>
        /// Gets the passengers of the vehicle.
        /// </summary>
        IReadOnlyCollection<IEntity> Passengers { get; }

        /// <summary>
        /// Adds a passenger to the vehicle.
        /// </summary>
        /// <param name="passenger">The passenger to add.</param>
        /// <returns><b>True</b> if the passenger was added; otherwise, <b>false</b>.</returns>
        Task<bool> AddPassengerAsync(IEntity passenger);

        /// <summary>
        /// Removes a passenger from the vehicle.
        /// </summary>
        /// <param name="passenger">The passenger to remove.</param>
        /// <returns><b>True</b> if the passenger was removed; otherwise, <b>false</b>.</returns>
        Task<bool> RemovePassengerAsync(IEntity passenger);

        /// <summary>
        /// Destroys the vehicle.
        /// </summary>
        Task DestroyAsync();
    }
}