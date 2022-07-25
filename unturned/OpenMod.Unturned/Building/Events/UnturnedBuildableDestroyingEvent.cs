using OpenMod.Extensions.Games.Abstractions.Building;
using OpenMod.Unturned.Players;
using SDG.Unturned;

namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedBuildableDestroyingEvent : UnturnedBuildableDamagingEvent, IBuildableDestroyingEvent
    {
        public UnturnedBuildableDestroyingEvent(UnturnedBuildable buildable, ushort damageAmount,
            EDamageOrigin damageOrigin, UnturnedPlayer? instigator) : base(buildable,
            damageAmount, damageOrigin, instigator)
        {
        }
    }
}
