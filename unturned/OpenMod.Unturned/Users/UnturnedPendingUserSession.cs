using OpenMod.API;
using OpenMod.Core.Users;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenMod.Unturned.Users
{
    public class UnturnedPendingUserSession : UserSessionBase
    {
        [OpenModInternal]
        public UnturnedPendingUserSession(UnturnedPendingUser user) : base(user)
        {
            SessionStartTime = DateTime.Now;
            SessionData = new Dictionary<string, object?>();
        }

        public override Task DisconnectAsync(string reason = "")
        {
            SessionEndTime = DateTime.Now;
            Provider.reject(((UnturnedPendingUser)User).SteamId, ESteamRejection.PLUGIN, reason ?? string.Empty);
            return Task.CompletedTask;
        }

        public void OnSessionEnd()
        {
            SessionEndTime = DateTime.Now;
        }
    }
}