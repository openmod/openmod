namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedBarricadeDeployEvent : UnturnedBuildableDeployEvent, IUnturnedBarricadeEvent
    {
        public new UnturnedBarricadeBuildable Buildable => (UnturnedBarricadeBuildable)base.Buildable;

        public UnturnedBarricadeDeployEvent(UnturnedBarricadeBuildable buildable) : base(buildable)
        {
        }
    }
}
