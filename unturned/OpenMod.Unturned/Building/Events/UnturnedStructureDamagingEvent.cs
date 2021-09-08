using OpenMod.Unturned.Players;
using SDG.Unturned;

namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedStructureDamagingEvent : UnturnedBuildableDamagingEvent, IUnturnedStructureEvent
    {
        public new UnturnedStructureBuildable Buildable => (UnturnedStructureBuildable)base.Buildable;

        public UnturnedStructureDamagingEvent(UnturnedStructureBuildable buildable, ushort damageAmount,
            EDamageOrigin damageOrigin, UnturnedPlayer? instigator) : base(buildable,
            damageAmount, damageOrigin, instigator)
        {
        }
    }
}
