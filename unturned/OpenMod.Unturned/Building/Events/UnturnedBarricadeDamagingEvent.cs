using OpenMod.Unturned.Players;
using SDG.Unturned;

namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedBarricadeDamagingEvent : UnturnedBuildableDamagingEvent, IUnturnedBarricadeEvent
    {
        public new UnturnedBarricadeBuildable Buildable => (UnturnedBarricadeBuildable)base.Buildable;

        public UnturnedBarricadeDamagingEvent(UnturnedBarricadeBuildable buildable, ushort damageAmount,
            EDamageOrigin damageOrigin, UnturnedPlayer? instigator) : base(buildable,
            damageAmount, damageOrigin, instigator)
        {
        }
    }
}
