namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedBarricadeSalvageEvent : UnturnedBuildableSalvageEvent, IUnturnedBarricadeEvent
    {
        public new UnturnedBarricadeBuildable Buildable => (UnturnedBarricadeBuildable)base.Buildable;

        public UnturnedBarricadeSalvageEvent(UnturnedBarricadeBuildable buildable) : base(buildable)
        {
        }
    }
}
