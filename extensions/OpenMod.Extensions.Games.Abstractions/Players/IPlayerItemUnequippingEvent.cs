using OpenMod.API.Eventing;
using OpenMod.Extensions.Games.Abstractions.Items;

namespace OpenMod.Extensions.Games.Abstractions.Players
{
    /// <summary>
    /// The event that is triggered when a player is unequipping an item.
    /// </summary>
    public interface IPlayerItemUnequippingEvent : IPlayerEvent, IItemEvent, ICancellableEvent
    {
    }
}