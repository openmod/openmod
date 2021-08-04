using OpenMod.Unturned.Players;
using SDG.Unturned;

namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedBarricadeDestroyingEvent : UnturnedBuildableDestroyingEvent, IUnturnedBarricadeEvent
    {
        public new UnturnedBarricadeBuildable Buildable => (UnturnedBarricadeBuildable)base.Buildable;

        public UnturnedBarricadeDestroyingEvent(UnturnedBarricadeBuildable buildable, ushort damageAmount,
            EDamageOrigin damageOrigin, UnturnedPlayer? instigator) : base(buildable,
            damageAmount, damageOrigin, instigator)
        {
        }
    }
}
