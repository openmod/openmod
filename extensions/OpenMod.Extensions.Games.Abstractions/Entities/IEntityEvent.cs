using OpenMod.API.Eventing;

namespace OpenMod.Extensions.Games.Abstractions.Entities
{
    public interface IEntityEvent : IEvent
    {
        IEntity Entity { get; }
    }
}