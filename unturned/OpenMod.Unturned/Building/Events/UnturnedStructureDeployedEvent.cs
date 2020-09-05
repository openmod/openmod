namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedStructureDeployedEvent : UnturnedBuildableDeployedEvent, IUnturnedStructureEvent
    {
        public new UnturnedStructureBuildable Buildable => (UnturnedStructureBuildable)base.Buildable;

        public UnturnedStructureDeployedEvent(UnturnedStructureBuildable buildable) : base(buildable)
        {
        }
    }
}
