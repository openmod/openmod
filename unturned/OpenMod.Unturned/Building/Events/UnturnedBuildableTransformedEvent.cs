using OpenMod.Unturned.Players;
using Steamworks;

namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedBuildableTransformedEvent : UnturnedBuildableEvent
    {
        public CSteamID InstigatorSteamId { get; }

        public UnturnedPlayer? Instigator { get; }

        public UnturnedBuildableTransformedEvent(UnturnedBuildable buildable, CSteamID instigatorSteamId,
            UnturnedPlayer? instigator) : base(buildable)
        {
            InstigatorSteamId = instigatorSteamId;
            Instigator = instigator;
        }
    }
}
