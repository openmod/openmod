using System;
using OpenMod.API.Eventing;
using OpenMod.Unturned.Players;
using Steamworks;

namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedSignModifyingEvent : UnturnedBuildableEvent, IUnturnedBarricadeEvent, ICancellableEvent
    {
        public bool IsCancelled { get; set; }

        [Obsolete("Use Instigator.SteamId")]
        public CSteamID InstigatorSteamId { get { return Instigator?.SteamId ?? CSteamID.Nil; } }

        public UnturnedPlayer? Instigator { get; }

        public string Text { get; set; }

        public new UnturnedBarricadeBuildable Buildable => (UnturnedBarricadeBuildable)base.Buildable;

        public UnturnedSignModifyingEvent(UnturnedBuildable buildable, UnturnedPlayer? instigator, string text)
            : base(buildable)
        {
            Instigator = instigator;
            Text = text;
        }
    }
}
