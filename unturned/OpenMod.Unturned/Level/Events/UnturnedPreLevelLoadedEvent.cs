using OpenMod.Core.Eventing;

namespace OpenMod.Unturned.Level.Events
{
    /// <summary>
    /// The event that is triggered before UnturnedLevelLoadedEvent.
    /// </summary>
    public class UnturnedPreLevelLoadedEvent : Event
    {
        public int Level { get; set; }

        public UnturnedPreLevelLoadedEvent(int level)
        {
            Level = level;
        }
    }
}
