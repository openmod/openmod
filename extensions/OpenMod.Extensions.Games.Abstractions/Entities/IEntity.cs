using System.Numerics;
using System.Threading.Tasks;

namespace OpenMod.Extensions.Games.Abstractions.Entities
{
    public interface IEntity : IGameObject
    {
        IEntityAsset Asset { get; }

        IEntityState State { get; }

        string EntityInstanceId { get; }

        Task<bool> SetPositionAsync(Vector3 position);

        Task<bool> SetPositionAsync(Vector3 position, Vector3 rotation);
    }
}