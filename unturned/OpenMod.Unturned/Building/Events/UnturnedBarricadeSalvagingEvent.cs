using OpenMod.Unturned.Players;

namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedBarricadeSalvagingEvent : UnturnedBuildableSalvagingEvent, IUnturnedBarricadeEvent
    {
        public new UnturnedBarricadeBuildable Buildable => (UnturnedBarricadeBuildable)base.Buildable;

        public UnturnedBarricadeSalvagingEvent(UnturnedBarricadeBuildable buildable, UnturnedPlayer instigator)
            : base(buildable, instigator)
        {
        }
    }
}
