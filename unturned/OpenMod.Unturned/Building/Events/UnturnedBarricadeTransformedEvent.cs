using OpenMod.Unturned.Players;
using Steamworks;

namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedBarricadeTransformedEvent : UnturnedBuildableTransformedEvent, IUnturnedBarricadeEvent
    {
        public new UnturnedBarricadeBuildable Buildable => (UnturnedBarricadeBuildable) base.Buildable;

        public UnturnedBarricadeTransformedEvent(UnturnedBuildable buildable, CSteamID instigatorSteamId,
            UnturnedPlayer? instigator) : base(buildable, instigatorSteamId, instigator)
        {
        }
    }
}
