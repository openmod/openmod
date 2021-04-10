using OpenMod.API.Eventing;
using System;

namespace OpenMod.Core.Users.Events
{
    public interface IUserBanningEvent : IUserEvent, ICancellableEvent
    {
        /// <summary>
        /// Gets the punisher Id.
        /// </summary>
        public string PunisherId { get; }

        /// <summary>
        /// Gets the ban reason.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Gets the ban duration.
        /// </summary>
        public DateTime Duration { get; set; }
    }
}
