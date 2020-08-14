using OpenMod.Extensions.Games.Abstractions.Transforms;

namespace OpenMod.Extensions.Games.Abstractions.Entities
{
    public interface IEntity : IGameTransform
    {
        IEntityAsset Asset { get; }

        IEntityState State { get; }

        string EntityInstanceId { get; }
    }
}