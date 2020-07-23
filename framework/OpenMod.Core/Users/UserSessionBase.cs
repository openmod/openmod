using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.API.Users;

namespace OpenMod.Core.Users
{
    public abstract class UserSessionBase : IUserSession
    {
        public virtual DateTime? SessionStartTime { get; protected set; }

        public virtual DateTime? SessionEndTime { get; protected set; }

        public virtual Dictionary<string, object> SessionData { get; protected set; }

        public abstract Task DisconnectAsync(string reason = "");
    }
}