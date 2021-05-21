using OpenMod.API.Eventing;
using OpenMod.Unturned.Players;

using Steamworks;

namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedSignModifyingEvent : UnturnedBuildableEvent, IUnturnedBarricadeEvent, ICancellableEvent
    {
        public bool IsCancelled { get; set; }

        public CSteamID InstigatorSteamId { get; }

        public UnturnedPlayer? Instigator { get; }

        public string Text { get; set; }

        public new UnturnedBarricadeBuildable Buildable => (UnturnedBarricadeBuildable) base.Buildable;

        public UnturnedSignModifyingEvent(UnturnedBuildable buildable, UnturnedPlayer instigator, CSteamID instigatorSteamId, string text)
            : base(buildable)
        {
            Instigator = instigator;
            InstigatorSteamId = instigatorSteamId;
            Text = text;
        }
    }
}
