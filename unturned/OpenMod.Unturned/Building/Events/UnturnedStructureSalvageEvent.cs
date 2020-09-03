namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedStructureSalvageEvent : UnturnedBuildableSalvageEvent, IUnturnedStructureEvent
    {
        public new UnturnedStructureBuildable Buildable => (UnturnedStructureBuildable)base.Buildable;

        public UnturnedStructureSalvageEvent(UnturnedStructureBuildable buildable) : base(buildable)
        {
        }
    }
}
