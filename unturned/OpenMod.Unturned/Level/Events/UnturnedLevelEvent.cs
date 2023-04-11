using OpenMod.Core.Eventing;

namespace OpenMod.Unturned.Level.Events
{
    public class UnturnedLevelEvent : Event
    {
        public int Level { get; set; }

        public UnturnedLevelEvent(int level)
        {
            Level = level;
        }
    }
}
