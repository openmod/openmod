using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.API.Ioc;

namespace OpenMod.Extensions.Games.Abstractions.Building
{
    /// <summary>
    /// The service for querying buildables and buildable assets.
    /// </summary>
    [Service]
    public interface IBuildableDirectory
    {
        /// <summary>
        /// Gets all buildable assets.
        /// </summary>
        Task<IReadOnlyCollection<IBuildableAsset>> GetBuildableAssetsAsync();

        /// <summary>
        /// Gets all placed buildables.
        /// </summary>
        Task<IReadOnlyCollection<IBuildable>> GetBuildablesAsync();
    }
}