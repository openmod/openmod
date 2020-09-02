using OpenMod.Core.Eventing;

namespace OpenMod.Unturned.Environment.Events
{
    public class UnturnedDayNightUpdateEvent : Event
    {
        public bool IsDayTime { get; }

        public UnturnedDayNightUpdateEvent(bool isDayTime)
        {
            IsDayTime = isDayTime;
        }
    }
}
