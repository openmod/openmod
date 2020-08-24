using OpenMod.Core.Eventing;

namespace OpenMod.Unturned.Events.Environment
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
