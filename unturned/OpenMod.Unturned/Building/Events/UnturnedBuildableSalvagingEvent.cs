using OpenMod.API.Eventing;

namespace OpenMod.Unturned.Building.Events
{
    public abstract class UnturnedBuildableSalvagingEvent : UnturnedBuildableEvent, ICancellableEvent
    {
        public bool IsCancelled { get; set; }

        protected UnturnedBuildableSalvagingEvent(UnturnedBuildable buildable) : base(buildable)
        {
        }
    }
}
