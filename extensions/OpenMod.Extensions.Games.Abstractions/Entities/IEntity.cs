using OpenMod.Extensions.Games.Abstractions.Transforms;

namespace OpenMod.Extensions.Games.Abstractions.Entities
{
    public interface IEntity : IGameTransform
    {
        string Name { get; }

        string EntityType { get; }
   
        string EntityInstanceId { get; }
    }
}