using System.Numerics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using OpenMod.API.Ioc;

namespace OpenMod.Extensions.Games.Abstractions.Building
{
    /// <summary>
    /// The service for spawning buildables.
    /// </summary>
    [Service]
    public interface IBuildableSpawner
    {
        /// <summary>
        /// Spawns a buildable at the given position.
        /// </summary>
        /// <param name="position">The position to spawn the buildable at.</param>
        /// <param name="buildableAssetId">The ID of the buildable asset.</param>
        /// <param name="state">The optional state for the buildable.</param>
        /// <returns><b>The created buildable</b> if successful; otherwise, <b>>null</b>.</returns>
        [CanBeNull]
        Task<IBuildable> SpawnBuildableAsync(Vector3 position, string buildableAssetId, [CanBeNull] IBuildableAsset state = null);
    }
}