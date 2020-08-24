using OpenMod.Core.Eventing;

namespace OpenMod.Unturned.Events.Environment
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
