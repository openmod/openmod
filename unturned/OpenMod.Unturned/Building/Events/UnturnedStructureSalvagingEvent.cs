using OpenMod.Unturned.Players;
using Steamworks;

namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedStructureSalvagingEvent : UnturnedBuildableSalvagingEvent, IUnturnedStructureEvent
    {
        public new UnturnedStructureBuildable Buildable => (UnturnedStructureBuildable)base.Buildable;

        public UnturnedStructureSalvagingEvent(UnturnedBuildable buildable, UnturnedPlayer instigator,
            CSteamID instigatorSteamId) : base(buildable, instigator, instigatorSteamId)
        {
        }
    }
}
