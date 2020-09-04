using OpenMod.Core.Eventing;

namespace OpenMod.Unturned.Environment.Events
{
    public class UnturnedDayNightUpdatedEvent : Event
    {
        public bool IsDayTime { get; }

        public UnturnedDayNightUpdatedEvent(bool isDayTime)
        {
            IsDayTime = isDayTime;
        }
    }
}
