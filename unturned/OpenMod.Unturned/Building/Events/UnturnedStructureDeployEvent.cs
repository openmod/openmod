namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedStructureDeployEvent : UnturnedBuildableDeployEvent, IUnturnedStructureEvent
    {
        public new UnturnedStructureBuildable Buildable => (UnturnedStructureBuildable)base.Buildable;

        public UnturnedStructureDeployEvent(UnturnedStructureBuildable buildable) : base(buildable)
        {
        }
    }
}
