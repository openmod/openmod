using System;
using OpenMod.Extensions.Games.Abstractions.Building;
using OpenMod.Unturned.Players;
using Steamworks;

namespace OpenMod.Unturned.Building.Events
{
    public class UnturnedBuildableTransformedEvent : UnturnedBuildableEvent, IBuildableTransformedEvent
    {
        [Obsolete("Use Instigator.SteamId")]
        public CSteamID InstigatorSteamId { get { return Instigator?.SteamId ?? CSteamID.Nil; } }

        public UnturnedPlayer? Instigator { get; }

        public UnturnedBuildableTransformedEvent(UnturnedBuildable buildable, UnturnedPlayer? instigator) : base(buildable)
        {
            Instigator = instigator;
        }
    }
}
