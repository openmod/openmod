using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
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
        /// The asset of the vehicle. Cannot be null.
        /// </value>
        [NotNull]
        IVehicleAsset Asset { get; }

        /// <value>
        /// The state of the vehicle. Cannot be null.
        /// </value>
        [NotNull]
        IVehicleState State { get; }

        /// <value>
        /// The transform of the vehicle. Cannot be null.
        /// </value>
        [NotNull]
        IWorldTransform Transform { get; }

        /// <summary>
        /// The unique instance ID of the vehicle. Cannot be null.
        /// </summary>
        [NotNull]
        string VehicleInstanceId { get; }

        /// <summary>
        /// The driver of the vehicle. Can be null.
        /// </summary>
        [CanBeNull]
        IEntity Driver { get; }

        /// <summary>
        /// List of the passengers of this vehicle. Cannot be null but empty.
        /// </summary>
        [NotNull]
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