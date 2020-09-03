using OpenMod.API.Eventing;

namespace OpenMod.Unturned.Building.Events
{
    public abstract class UnturnedBuildableSalvageEvent : UnturnedBuildableEvent, ICancellableEvent
    {
        public bool IsCancelled { get; set; }

        protected UnturnedBuildableSalvageEvent(UnturnedBuildable buildable) : base(buildable)
        {
        }
    }
}
