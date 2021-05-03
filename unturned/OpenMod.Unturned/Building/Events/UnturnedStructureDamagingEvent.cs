using OpenMod.Unturned.Players;
using SDG.Unturned;
using Steamworks;

namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedStructureDamagingEvent : UnturnedBuildableDamagingEvent, IUnturnedStructureEvent
    {
        public new UnturnedStructureBuildable Buildable => (UnturnedStructureBuildable)base.Buildable;

        public UnturnedStructureDamagingEvent(UnturnedStructureBuildable buildable, ushort damageAmount,
            EDamageOrigin damageOrigin, UnturnedPlayer? instigator, CSteamID instigatorId) : base(buildable,
            damageAmount, damageOrigin, instigator, instigatorId)
        {
        }
    }
}
