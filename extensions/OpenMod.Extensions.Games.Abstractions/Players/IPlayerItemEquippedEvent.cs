using OpenMod.Extensions.Games.Abstractions.Items;

namespace OpenMod.Extensions.Games.Abstractions.Players
{
    /// <summary>
    /// The event that is triggered when a player has equipped an item.
    /// </summary>
    public interface IPlayerItemEquippedEvent : IPlayerEvent, IItemEvent
    {
    }
}