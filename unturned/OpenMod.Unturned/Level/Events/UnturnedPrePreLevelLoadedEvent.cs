using OpenMod.Core.Eventing;

namespace OpenMod.Unturned.Level.Events
{
    /// <summary>
    /// The event that is triggered before UnturnedPreLevelLoadedEvent.
    /// </summary>
    public class UnturnedPrePreLevelLoadedEvent : Event
    {
        public int Level { get; set; }

        public UnturnedPrePreLevelLoadedEvent(int level)
        {
            Level = level;
        }
    }
}
