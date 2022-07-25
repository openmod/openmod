using OpenMod.Unturned.Players;
using SDG.Unturned;

namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedStructureDestroyingEvent : UnturnedBuildableDestroyingEvent, IUnturnedStructureEvent
    {
        public new UnturnedStructureBuildable Buildable => (UnturnedStructureBuildable)base.Buildable;

        public UnturnedStructureDestroyingEvent(UnturnedStructureBuildable buildable, ushort damageAmount,
            EDamageOrigin damageOrigin, UnturnedPlayer? instigator) : base(buildable,
            damageAmount, damageOrigin, instigator)
        {
        }
    }
}
