using OpenMod.Core.Eventing;

namespace OpenMod.Unturned.Environment.Events
{
    public class UnturnedMoonUpdateEvent : Event
    {
        public bool IsFullMoon { get; }

        public UnturnedMoonUpdateEvent(bool isFullMoon)
        {
            IsFullMoon = isFullMoon;
        }
    }
}
