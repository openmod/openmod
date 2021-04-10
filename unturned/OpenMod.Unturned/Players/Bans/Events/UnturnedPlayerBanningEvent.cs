using OpenMod.API.Eventing;
using OpenMod.Core.Eventing;
using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Unturned.Events;
using Steamworks;
using System;

namespace OpenMod.Unturned.Players.Bans.Events
{
    public class UnturnedPlayerBanningEvent : UnturnedPlayerEvent, IPlayerBanningEvent
    {
        public string InstigatorId { get; }

        public uint IPToBan { get; }

        public string Reason { get; set; }

        public DateTime Duration { get; set; }

        public bool IsCancelled { get; set; }

        public UnturnedPlayerBanningEvent(UnturnedPlayer playerToBan, string instigatorId, uint ipToBan, string reason, uint duration) : base(playerToBan)
        {
            InstigatorId = instigatorId;
            IPToBan = ipToBan;
            Reason = reason;
            Duration = DateTime.Now.AddSeconds(duration);
        }
    }
}
