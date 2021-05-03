using OpenMod.Unturned.Players;
using SDG.Unturned;
using Steamworks;

namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedStructureDestroyingEvent : UnturnedBuildableDestroyingEvent, IUnturnedStructureEvent
    {
        public new UnturnedStructureBuildable Buildable => (UnturnedStructureBuildable)base.Buildable;

        public UnturnedStructureDestroyingEvent(UnturnedStructureBuildable buildable, ushort damageAmount,
            EDamageOrigin damageOrigin, UnturnedPlayer? instigator, CSteamID instigatorId) : base(buildable,
            damageAmount, damageOrigin, instigator, instigatorId)
        {
        }
    }
}
