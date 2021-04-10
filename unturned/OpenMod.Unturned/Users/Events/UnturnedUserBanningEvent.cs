using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.Core.Users.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenMod.Unturned.Users.Events
{
    public class UnturnedUserBanningEvent : UnturnedUserEvent, IUserBanningEvent
    {
        public UnturnedUserBanningEvent(UnturnedUser user, string punisherId, string reason, DateTime duration) : base(user)
        {
            PunisherId = punisherId;
            Reason = reason;
            Duration = duration;
        }

        public string PunisherId { get; }
        public string Reason { get; set; }
        public DateTime Duration { get; set; }
        public bool IsCancelled { get; set; }
    }
}
