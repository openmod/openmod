using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using OpenMod.API;
using OpenMod.API.Users;
using OpenMod.Core.Users;
using OpenMod.Unturned.Entities;
using SDG.Unturned;

namespace OpenMod.Unturned.Users
{
    public class UnturnedPlayerSession : UserSessionBase
    {
        public UnturnedPlayer Player { get; }

        [OpenModInternal]
        public UnturnedPlayerSession(UnturnedPlayer player, IUserSession baseSession = null)
        {
            Player = player;
            SessionData = baseSession?.SessionData ?? new Dictionary<string, object>();
            SessionStartTime = baseSession?.SessionStartTime ?? DateTime.Now;
        }

        public override Task DisconnectAsync(string reason = "")
        {
            async UniTask Task()
            {
                await UniTask.SwitchToMainThread();
                SessionEndTime = DateTime.Now;
                Provider.kick(Player.SteamId, reason ?? string.Empty);
            }

            return Task().AsTask();
        }

        public void OnSessionEnd()
        {
            SessionEndTime = DateTime.Now;
        }
    }
}