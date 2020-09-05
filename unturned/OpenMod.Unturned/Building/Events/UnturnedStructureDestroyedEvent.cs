namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedStructureDestroyedEvent : UnturnedBuildableDestroyedEvent, IUnturnedStructureEvent
    {
        public new UnturnedStructureBuildable Buildable => (UnturnedStructureBuildable)base.Buildable;

        public UnturnedStructureDestroyedEvent(UnturnedStructureBuildable buildable) : base(buildable)
        {
        }
    }
}
