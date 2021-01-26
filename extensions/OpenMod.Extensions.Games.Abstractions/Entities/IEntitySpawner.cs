using System.Numerics;
using System.Threading.Tasks;
using OpenMod.API.Ioc;

namespace OpenMod.Extensions.Games.Abstractions.Entities
{
    /// <summary>
    /// The service for spawning entities.
    /// </summary>
    [Service]
    public interface IEntitySpawner
    {
        /// <summary>
        /// Spawns an entity at the given position.
        /// </summary>
        /// <param name="position">The position to spawn the entity at.</param>
        /// <param name="entityAssetId">The ID of the entity asset.</param>
        /// <param name="state">The optional state for the entity to spawn.</param>
        /// <returns><b>The spawn entity</b> if successful; otherwise <b>null</b>.</returns>
        Task<IEntity?> SpawnEntityAsync(Vector3 position, string entityAssetId, IEntityState? state = null);
    }
}