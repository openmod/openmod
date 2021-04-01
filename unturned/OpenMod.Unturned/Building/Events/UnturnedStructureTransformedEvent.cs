using OpenMod.Unturned.Players;
using Steamworks;

namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedStructureTransformedEvent : UnturnedBuildableTransformedEvent, IUnturnedStructureEvent
    {
        public new UnturnedStructureBuildable Buildable => (UnturnedStructureBuildable) base.Buildable;

        public UnturnedStructureTransformedEvent(UnturnedBuildable buildable, CSteamID instigatorSteamId,
            UnturnedPlayer? instigator) : base(buildable, instigatorSteamId, instigator)
        {
        }
    }
}
