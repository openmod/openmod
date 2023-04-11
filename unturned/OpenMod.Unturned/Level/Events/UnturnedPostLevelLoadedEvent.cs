using OpenMod.Core.Eventing;

namespace OpenMod.Unturned.Level.Events
{
    /// <summary>
    /// The event that is triggered after UnturnedLevelLoadedEvent.
    /// </summary>
    public class UnturnedPostLevelLoadedEvent : Event
    {
        public int Level { get; set; }

        public UnturnedPostLevelLoadedEvent(int level)
        {
            Level = level;
        }
    }
}
