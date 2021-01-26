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
        /// <value>
        /// The asset of the vehicle.
        /// </value>
        IVehicleAsset Asset { get; }

        /// <value>
        /// The state of the vehicle.
        /// </value>
        IVehicleState State { get; }

        /// <value>
        /// The transform of the vehicle.
        /// </value>
        IWorldTransform Transform { get; }

        /// <summary>
        /// The unique instance ID of the vehicle.
        /// </summary>
        string VehicleInstanceId { get; }

        /// <summary>
        /// The driver of the vehicle.
        /// </summary>
        IEntity? Driver { get; }

        /// <summary>
        /// List of the passengers of this vehicle.
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