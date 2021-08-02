using OpenMod.API.Eventing;
using OpenMod.Unturned.Players;

namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedStorageOpeningEvent : UnturnedBuildableEvent, IUnturnedBarricadeEvent, ICancellableEvent
    {
        public bool IsCancelled { get; set; }

        public UnturnedPlayer? Instigator { get; }

        public new UnturnedBarricadeBuildable Buildable => (UnturnedBarricadeBuildable)base.Buildable;

        public UnturnedStorageOpeningEvent(UnturnedBuildable buildable, UnturnedPlayer? instigator) : base(buildable)
        {
            Instigator = instigator;
        }
    }
}
