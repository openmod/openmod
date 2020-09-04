using OpenMod.Core.Eventing;

namespace OpenMod.Unturned.Environment.Events
{
    public class UnturnedDayNightUpdatedEvent : Event
    {
        public WorldTime WorldTime { get; }

        public bool IsFullMoon { get; }

        public UnturnedDayNightUpdatedEvent(WorldTime worldTime, bool isFullMoon)
        {
            WorldTime = worldTime;
            IsFullMoon = isFullMoon;
        }
    }
}
