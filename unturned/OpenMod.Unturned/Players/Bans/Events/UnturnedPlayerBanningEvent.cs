using OpenMod.API.Eventing;
using OpenMod.Core.Eventing;
using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Unturned.Events;
using Steamworks;
using System;
using System.Net;

namespace OpenMod.Unturned.Players.Bans.Events
{
    public class UnturnedPlayerBanningEvent : UnturnedPlayerEvent, IBanningPlayerEvent
    {
        public string InstigatorId { get; }

        public string InstigatorType { get; }

        public IPAddress? AddressToBan { get; }

        public string? Reason { get; set; }

        public TimeSpan Duration { get; set; }

        public bool IsCancelled { get; set; }

        [Obsolete("Use " + nameof(AddressToBan) + " instead")]
        public uint IPToBan
        {
            get { return AddressToBan == null ? 0 : (uint) IPAddress.NetworkToHostOrder(AddressToBan.Address); }
        }

        public UnturnedPlayerBanningEvent(
            UnturnedPlayer playerToBan,
            string instigatorId,
            string instigatorType,
            IPAddress? addressBan,
            string? reason,
            TimeSpan duration)
            : base(playerToBan)
        {
            InstigatorId = instigatorId;
            InstigatorType = instigatorType;
            AddressToBan = addressBan;
            Reason = reason;
            Duration = duration;
        }
    }
}
