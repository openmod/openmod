using System.Numerics;
using System.Threading.Tasks;

namespace OpenMod.Extensions.Games.Abstractions.Entities
{
    /// <summary>
    /// Represents an entity such as players, NPCs, cars, etc.
    /// </summary>
    public interface IEntity : IGameObject
    {
        /// <summary>
        /// Gets the asset of the entity.
        /// </summary>
        IEntityAsset Asset { get; }

        /// <summary>
        /// Gets the state of the entity.
        /// </summary>
        IEntityState State { get; }

        /// <summary>
        /// Gets the unique instance ID of the entity.
        /// </summary>
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
        Task<bool> SetPositionAsync(Vector3 position, Quaternion rotation);
    }
}