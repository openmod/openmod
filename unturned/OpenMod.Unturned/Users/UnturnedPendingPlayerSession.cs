using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.API;
using OpenMod.Core.Users;
using SDG.Unturned;

namespace OpenMod.Unturned.Users
{
    public class UnturnedPendingPlayerSession : UserSessionBase
    {
        public UnturnedPendingPlayer Player { get; }

        [OpenModInternal]
        public UnturnedPendingPlayerSession(UnturnedPendingPlayer player)
        {
            Player = player;
            SessionStartTime = DateTime.Now;
            SessionData = new Dictionary<string, object>();
        }

        public override Task DisconnectAsync(string reason = "")
        {
            SessionEndTime = DateTime.Now;
            Provider.reject(Player.SteamId, ESteamRejection.PLUGIN, reason ?? string.Empty);
            return Task.CompletedTask;
        }

        public void OnSessionEnd()
        {
            SessionEndTime = DateTime.Now;
        }
    }
}