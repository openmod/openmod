using JetBrains.Annotations;

namespace OpenMod.Extensions.Games.Abstractions.Entities
{
    public interface IEntityState
    {
        [CanBeNull]
        byte[] StateData { get; }
    }
}