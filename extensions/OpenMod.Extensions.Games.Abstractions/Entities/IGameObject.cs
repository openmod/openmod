using OpenMod.Extensions.Games.Abstractions.Transforms;

namespace OpenMod.Extensions.Games.Abstractions.Entities
{
    public interface IGameObject
    {
        IWorldTransform Transform { get; }
    }
}