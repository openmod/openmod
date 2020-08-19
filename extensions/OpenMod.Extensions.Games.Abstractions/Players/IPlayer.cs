using OpenMod.Extensions.Games.Abstractions.Entities;

namespace OpenMod.Extensions.Games.Abstractions.Players
{
    public interface IPlayer : IEntity
    {
        string Stance { get; }
    }
}