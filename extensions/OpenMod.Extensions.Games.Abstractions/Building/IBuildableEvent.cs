using OpenMod.API.Eventing;

namespace OpenMod.Extensions.Games.Abstractions.Building
{
    public interface IBuildableEvent : IEvent
    {
        IBuildable Buildable { get; }
    }
}