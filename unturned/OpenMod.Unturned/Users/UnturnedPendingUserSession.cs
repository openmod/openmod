using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.Core.Users;
using SDG.Unturned;

namespace OpenMod.Unturned.Users
{
    public class UnturnedPendingUserSession : UserSessionBase
    {
        public UnturnedPendingUser User { get; }

        public UnturnedPendingUserSession(UnturnedPendingUser user)
        {
            User = user;
            SessionStartTime = DateTime.Now;
            SessionData = new Dictionary<string, object>();
        }

        public override Task DisconnectAsync(string reason = "")
        {
            SessionEndTime = DateTime.Now;
            Provider.reject(User.SteamId, ESteamRejection.PLUGIN, reason ?? string.Empty);
            return Task.CompletedTask;
        }

        public void OnSessionEnd()
        {
            SessionEndTime = DateTime.Now;
        }
    }
}