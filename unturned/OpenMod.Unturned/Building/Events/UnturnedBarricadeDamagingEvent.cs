using OpenMod.Unturned.Players;
using SDG.Unturned;
using Steamworks;

namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedBarricadeDamagingEvent : UnturnedBuildableDamagingEvent, IUnturnedBarricadeEvent
    {
        public new UnturnedBarricadeBuildable Buildable => (UnturnedBarricadeBuildable)base.Buildable;

        public UnturnedBarricadeDamagingEvent(UnturnedBarricadeBuildable buildable, ushort damageAmount,
            EDamageOrigin damageOrigin, UnturnedPlayer? instigator, CSteamID instigatorId) : base(buildable,
            damageAmount, damageOrigin, instigator, instigatorId)
        {
        }
    }
}
