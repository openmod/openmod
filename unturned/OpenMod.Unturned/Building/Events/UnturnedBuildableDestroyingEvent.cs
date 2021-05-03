using OpenMod.Unturned.Players;
using SDG.Unturned;
using Steamworks;

namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedBuildableDestroyingEvent : UnturnedBuildableDamagingEvent
    {
        public UnturnedBuildableDestroyingEvent(UnturnedBuildable buildable, ushort damageAmount,
            EDamageOrigin damageOrigin, UnturnedPlayer? instigator, CSteamID instigatorId) : base(buildable,
            damageAmount, damageOrigin, instigator, instigatorId)
        {
        }
    }
}
