using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.API.Ioc;

namespace OpenMod.Extensions.Games.Abstractions.Entities
{
    /// <summary>
    /// The service for querying entities and entity assets.
    /// </summary>
    [Service]
    public interface IEntityDirectory
    {
        /// <summary>
        /// Gets all entities.
        /// </summary>
        Task<IReadOnlyCollection<IEntity>> GetEntitiesAsync();

        /// <summary>
        /// Gets all entity assets.
        /// </summary>
        Task<IReadOnlyCollection<IEntityAsset>> GetEntityAssetsAsync();
    }
}