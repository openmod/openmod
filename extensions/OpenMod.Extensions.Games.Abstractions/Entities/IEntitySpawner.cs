using System.Numerics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using OpenMod.API.Ioc;

namespace OpenMod.Extensions.Games.Abstractions.Entities
{
    [Service]
    public interface IEntitySpawner
    {
        [CanBeNull]
        Task<IEntity> SpawnEntityAsync(Vector3 position, string entityId, [CanBeNull] IEntityState state = null);
    }
}