namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedBarricadeDestroyedEvent : UnturnedBuildableDestroyedEvent, IUnturnedBarricadeEvent
    {
        public new UnturnedBarricadeBuildable Buildable => (UnturnedBarricadeBuildable)base.Buildable;

        public UnturnedBarricadeDestroyedEvent(UnturnedBarricadeBuildable buildable) : base(buildable)
        {
        }
    }
}
