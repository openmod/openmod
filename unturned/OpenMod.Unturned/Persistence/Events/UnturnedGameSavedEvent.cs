using OpenMod.Core.Eventing;
using OpenMod.Extensions.Games.Abstractions.Persistence;

namespace OpenMod.Unturned.Persistence.Events
{
    /// <summary>
    /// The event that is triggered when the game performs a save.
    /// </summary>
    public class UnturnedGameSavedEvent : Event, IGameSaveEvent
    {
    }
}