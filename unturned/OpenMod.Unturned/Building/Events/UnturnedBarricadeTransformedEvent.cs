namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedBarricadeTransformedEvent : UnturnedBuildableTransformedEvent, IUnturnedBarricadeEvent
    {
        public new UnturnedBarricadeBuildable Buildable => (UnturnedBarricadeBuildable) base.Buildable;

        public UnturnedBarricadeTransformedEvent(UnturnedBarricadeBuildable buildable) : base(buildable)
        {
        }
    }
}
