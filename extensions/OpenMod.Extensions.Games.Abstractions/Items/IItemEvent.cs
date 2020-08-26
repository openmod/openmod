using OpenMod.API.Eventing;

namespace OpenMod.Extensions.Games.Abstractions.Items
{
    public interface IItemEvent : IEvent
    {
        IItem Item { get; }
    }
}