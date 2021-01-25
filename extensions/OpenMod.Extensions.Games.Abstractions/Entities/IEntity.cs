using System.Numerics;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace OpenMod.Extensions.Games.Abstractions.Entities
{
    /// <summary>
    /// Represents an entity such as players, NPCs, cars, etc.
    /// </summary>
    public interface IEntity : IGameObject
    {
        /// <value>
        /// The asset of the entity. Cannot be null.
        /// </value>
        [NotNull]
        IEntityAsset Asset { get; }

        /// <value>
        /// The state of the entity. Cannot be null.
        /// </value>
        [NotNull]
        IEntityState State { get; }

        /// <value>
        /// The unique instance ID of the entity. Cannot be null.
        /// </value>
        [NotNull]
        string EntityInstanceId { get; }

        /// <summary>
        /// Sets the position of an entity.
        /// </summary>
        /// <param name="position">The position to set to.</param>
        /// <returns><b>True</b> if successful; otherwise, <b>false</b>.</returns>
        Task<bool> SetPositionAsync(Vector3 position);

        /// <summary>
        /// Sets the position and rotation of an entity.
        /// </summary>
        /// <param name="position">The position to set to.</param>
        /// <param name="rotation">The rotation to set to.</param>
        /// <returns><b>True</b> if successful; otherwise, <b>false</b>.</returns>
        Task<bool> SetPositionAsync(Vector3 position, Vector3 rotation);
    }
}