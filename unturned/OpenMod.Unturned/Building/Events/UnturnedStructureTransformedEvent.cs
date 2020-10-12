namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedStructureTransformedEvent : UnturnedBuildableTransformedEvent, IUnturnedStructureEvent
    {
        public new UnturnedStructureBuildable Buildable => (UnturnedStructureBuildable) base.Buildable;

        public UnturnedStructureTransformedEvent(UnturnedStructureBuildable buildable) : base(buildable)
        {
        }
    }
}
