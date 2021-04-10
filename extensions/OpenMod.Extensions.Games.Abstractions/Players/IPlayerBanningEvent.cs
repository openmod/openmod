using OpenMod.API.Eventing;
using System;

namespace OpenMod.Extensions.Games.Abstractions.Players
{
    public interface IPlayerBanningEvent : IPlayerEvent, ICancellableEvent
    {
        public string InstigatorId { get; }
        public string Reason { get; set; }
        public DateTime Duration { get; set; }
    }
}
