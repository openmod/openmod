namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedBarricadeDeployedEvent : UnturnedBuildableDeployedEvent, IUnturnedBarricadeEvent
    {
        public new UnturnedBarricadeBuildable Buildable => (UnturnedBarricadeBuildable)base.Buildable;

        public UnturnedBarricadeDeployedEvent(UnturnedBarricadeBuildable buildable) : base(buildable)
        {
        }
    }
}
