using OpenMod.Core.Eventing;

namespace OpenMod.Unturned.Environment.Events
{
    /// <summary>
    /// The event that is triggered when it has become day or night.
    /// </summary>
    public class UnturnedDayNightUpdatedEvent : Event
    {
        /// <summary>
        /// Gets the current world time.
        /// </summary>
        public WorldTime WorldTime { get; }

        /// <summary>
        /// Checks if its full moon.
        /// </summary>
        public bool IsFullMoon { get; }

        public UnturnedDayNightUpdatedEvent(WorldTime worldTime, bool isFullMoon)
        {
            WorldTime = worldTime;
            IsFullMoon = isFullMoon;
        }
    }
}
