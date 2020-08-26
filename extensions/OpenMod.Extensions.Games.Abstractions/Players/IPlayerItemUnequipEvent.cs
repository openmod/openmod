using OpenMod.API.Eventing;
using OpenMod.Extensions.Games.Abstractions.Items;

namespace OpenMod.Extensions.Games.Abstractions.Players
{
    public interface IPlayerItemUnequipEvent : IPlayerEvent, IItemEvent, ICancellableEvent
    {
    }
}