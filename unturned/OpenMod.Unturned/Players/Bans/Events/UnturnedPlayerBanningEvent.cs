using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Unturned.Events;
using System;
using System.Collections.Generic;
using System.Net;

namespace OpenMod.Unturned.Players.Bans.Events
{
    public class UnturnedPlayerBanningEvent : UnturnedPlayerEvent, IBanningPlayerEvent
    {
        public string InstigatorId { get; }

        public string InstigatorType { get; }

        public IPAddress? AddressToBan { get; }

        public IEnumerable<byte[]>? HwidsToBan { get; }

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
            IEnumerable<byte[]> hwidsToBan,
            string? reason,
            TimeSpan duration)
            : base(playerToBan)
        {
            InstigatorId = instigatorId;
            InstigatorType = instigatorType;
            AddressToBan = addressBan;
            HwidsToBan = hwidsToBan;
            Reason = reason;
            Duration = duration;
        }

        [Obsolete("Use the construtor with hwids instead")]
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
            HwidsToBan = null;
            Reason = reason;
            Duration = duration;
        }
    }
}
