using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.API.Users;

namespace OpenMod.Core.Users
{
    public abstract class UserSessionBase : IUserSession
    {
        private static readonly Dictionary<string, Dictionary<string, object?>> s_InstanceDataStore = new();
        public IUser User { get; }

        protected UserSessionBase(IUser user)
        {
            User = user;

            if (!s_InstanceDataStore.ContainsKey(user.Id))
            {
                s_InstanceDataStore.Add(user.Id, new Dictionary<string, object?>());
            }
        }

        public virtual DateTime? SessionStartTime { get; protected set; }

        public virtual DateTime? SessionEndTime { get; protected set; }

        public virtual Dictionary<string, object?> SessionData { get; protected set; } = new();

        public Dictionary<string, object?> InstanceData => s_InstanceDataStore[User.Id];

        public abstract Task DisconnectAsync(string reason = "");
    }
}