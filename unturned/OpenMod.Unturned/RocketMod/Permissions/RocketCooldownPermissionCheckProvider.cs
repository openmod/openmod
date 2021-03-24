using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using OpenMod.API.Permissions;
using OpenMod.Core.Helpers;
using OpenMod.Extensions.Games.Abstractions.Players;
using Rocket.API;
using Rocket.Core;

namespace OpenMod.Unturned.RocketMod.Permissions
{
    public class RocketCooldownPermissionCheckProvider : IPermissionCheckProvider
    {
        private readonly Dictionary<string, DateTime> m_Cooldowns;

        public RocketCooldownPermissionCheckProvider()
        {
            m_Cooldowns = new();
        }

        public bool SupportsActor(IPermissionActor actor)
        {
            return actor is IPlayerUser;
        }

        public Task<PermissionGrantResult> CheckPermissionAsync(IPermissionActor actor, string permission)
        {
            if (string.IsNullOrEmpty(permission))
            {
                return Task.FromResult(PermissionGrantResult.Default);
            }

            permission = permission.Trim();

            var rocketPlayer = new RocketPlayer(actor.Id, actor.DisplayName);
            var permissions  = R.Permissions.GetPermissions(rocketPlayer);

            foreach (var knownPermission in permissions)
            {
                if (!string.Equals(knownPermission.Name?.Trim(), permission, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (m_Cooldowns.ContainsKey(actor.Id))
                {
                    if (m_Cooldowns[actor.Id] > DateTime.UtcNow)
                    {
                        var timeLeft = m_Cooldowns[actor.Id] - DateTime.UtcNow;

                        // todo: there is no openmod.unturned.translations.yaml yet
                        AsyncHelper.RunSync(() => ((IPlayerUser) actor).PrintMessageAsync($"You must wait {timeLeft.TotalSeconds:0.##} seconds.", Color.Red));
                        return Task.FromResult(PermissionGrantResult.Deny);
                    }

                    m_Cooldowns.Remove(actor.Id);
                }

                m_Cooldowns.Add(actor.Id, DateTime.UtcNow + TimeSpan.FromSeconds(knownPermission.Cooldown));
            }

            return Task.FromResult(PermissionGrantResult.Default);
        }
    }
}
