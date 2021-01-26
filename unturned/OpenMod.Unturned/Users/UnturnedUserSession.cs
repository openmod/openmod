using Cysharp.Threading.Tasks;
using OpenMod.API;
using OpenMod.API.Users;
using OpenMod.Core.Users;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenMod.Unturned.Users
{
    public class UnturnedUserSession : UserSessionBase
    {
        [OpenModInternal]
        public UnturnedUserSession(UnturnedUser user, IUserSession? baseSession = null) : base(user)
        {
            SessionData = baseSession?.SessionData ?? new Dictionary<string, object?>();
            SessionStartTime = baseSession?.SessionStartTime ?? DateTime.Now;

            if (baseSession != null)
            {
                // copy instance data
                foreach (var key in baseSession.InstanceData.Keys)
                {
                    if (!InstanceData.ContainsKey(key))
                    {
                        InstanceData.Add(key, baseSession.InstanceData[key]);
                    }
                }
            }
        }

        public override Task DisconnectAsync(string reason = "")
        {
            async UniTask Task()
            {
                await UniTask.SwitchToMainThread();
                SessionEndTime = DateTime.Now;
                Provider.kick(((UnturnedUser)User).SteamId, reason ?? string.Empty);
            }

            return Task().AsTask();
        }

        public void OnSessionEnd()
        {
            SessionEndTime = DateTime.Now;
        }
    }
}