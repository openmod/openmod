using OpenMod.API.Eventing;
using OpenMod.Extensions.Games.Abstractions.Items;

namespace OpenMod.Extensions.Games.Abstractions.Players
{
    /// <summary>
    /// The event that is triggered when a player is equpping an item.
    /// </summary>
    public interface IPlayerItemEquippingEvent : IPlayerEvent, IItemEvent, ICancellableEvent
    {
    }
}