using OpenMod.Core.Eventing;

namespace OpenMod.Unturned.Environment.Events
{
    public class UnturnedMoonUpdatedEvent : Event
    {
        public bool IsFullMoon { get; }

        public UnturnedMoonUpdatedEvent(bool isFullMoon)
        {
            IsFullMoon = isFullMoon;
        }
    }
}
