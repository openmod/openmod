using OpenMod.API.Eventing;
using OpenMod.Unturned.Players;

namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedSignModifyingEvent : UnturnedBuildableEvent, IUnturnedBarricadeEvent, ICancellableEvent
    {
        public bool IsCancelled { get; set; }

        public UnturnedPlayer? Instigator { get; }

        public string Text { get; set; }

        public new UnturnedBarricadeBuildable Buildable => (UnturnedBarricadeBuildable)base.Buildable;

        public UnturnedSignModifyingEvent(UnturnedBuildable buildable, UnturnedPlayer? instigator, string text)
            : base(buildable)
        {
            Instigator = instigator;
            Text = text;
        }
    }
}
