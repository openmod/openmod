using OpenMod.Core.Eventing;

namespace OpenMod.Unturned.Environment.Events
{
    /// <summary>
    /// The event that is triggered when it has become day or night.
    /// </summary>
    public class UnturnedDayNightUpdatedEvent : Event
    {
        /// <value>
        /// The current time.
        /// </value>
        public WorldTime WorldTime { get; }

        /// <value>
        /// True if it is night and full moon.
        /// </value>
        public bool IsFullMoon { get; }

        public UnturnedDayNightUpdatedEvent(WorldTime worldTime, bool isFullMoon)
        {
            WorldTime = worldTime;
            IsFullMoon = isFullMoon;
        }
    }
}
