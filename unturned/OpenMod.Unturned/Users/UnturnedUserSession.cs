using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.API.Users;
using OpenMod.Core.Users;
using SDG.Unturned;

namespace OpenMod.Unturned.Users
{
    public class UnturnedUserSession : UserSessionBase
    {
        public UnturnedUser User { get; }

        public UnturnedUserSession(UnturnedUser user, IUserSession baseSession)
        {
            User = user;
            SessionData = baseSession?.SessionData ?? new Dictionary<string, object>();
            SessionStartTime = baseSession?.SessionStartTime ?? DateTime.Now;
        }

        public override Task DisconnectAsync(string reason = "")
        {
            SessionEndTime = DateTime.Now;
            Provider.kick(User.SteamId, reason ?? string.Empty);
            return Task.CompletedTask;
        }

        public void OnSessionEnd()
        {
            SessionEndTime = DateTime.Now;
        }
    }
}