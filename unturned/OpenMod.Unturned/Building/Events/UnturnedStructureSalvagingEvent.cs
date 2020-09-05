namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedStructureSalvagingEvent : UnturnedBuildableSalvagingEvent, IUnturnedStructureEvent
    {
        public new UnturnedStructureBuildable Buildable => (UnturnedStructureBuildable)base.Buildable;

        public UnturnedStructureSalvagingEvent(UnturnedStructureBuildable buildable) : base(buildable)
        {
        }
    }
}
