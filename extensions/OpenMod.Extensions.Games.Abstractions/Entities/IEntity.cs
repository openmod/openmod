using System.Numerics;
using System.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Transforms;

namespace OpenMod.Extensions.Games.Abstractions.Entities
{
    public interface IEntity
    {
        IEntityAsset Asset { get; }

        IEntityState State { get; }

        IWorldTransform Transform { get; }

        string EntityInstanceId { get; }

        Task<bool> SetPositionAsync(Vector3 position);
        Task<bool> SetPositionAsync(Vector3 position, Vector3 rotation);
    }
}