using System.Numerics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using OpenMod.API.Ioc;

namespace OpenMod.Extensions.Games.Abstractions.Building
{
    [Service]
    public interface IBuildableSpawner
    {
        [CanBeNull]
        Task<IBuildable> SpawnBuildableAsync(Vector3 position, string buildableId, [CanBeNull] IBuildableAsset state = null);
    }
}