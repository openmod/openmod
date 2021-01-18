using OpenMod.Unturned.Players;
using Steamworks;

namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedBarricadeSalvagingEvent : UnturnedBuildableSalvagingEvent, IUnturnedBarricadeEvent
    {
        public new UnturnedBarricadeBuildable Buildable => (UnturnedBarricadeBuildable)base.Buildable;

        public UnturnedBarricadeSalvagingEvent(UnturnedBuildable buildable, UnturnedPlayer instigator,
            CSteamID instigatorSteamId) : base(buildable, instigator, instigatorSteamId)
        {
        }
    }
}
