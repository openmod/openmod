﻿using OpenMod.API.Localization;
using OpenMod.API.Permissions;
using OpenMod.Core.Helpers;
using OpenMod.Extensions.Games.Abstractions.Players;
using Rocket.API;
using Rocket.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using OpenMod.API.Commands;

namespace OpenMod.Unturned.RocketMod.Permissions
{
    public class RocketCooldownPermissionCheckProvider : IPermissionCheckProvider
    {
        private readonly IOpenModHostStringLocalizer m_StringLocalizer;
        private readonly Dictionary<string, Dictionary<string, DateTime>> m_Cooldowns;

        public RocketCooldownPermissionCheckProvider(IOpenModHostStringLocalizer stringLocalizer)
        {
            m_StringLocalizer = stringLocalizer;
            m_Cooldowns = new Dictionary<string, Dictionary<string, DateTime>>();
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
            var permissions = R.Permissions.GetPermissions(rocketPlayer);

            if (m_Cooldowns.TryGetValue(actor.Id, out var permissionCooldown))
            {
                if (permissionCooldown.TryGetValue(permission, out var finishDate))
                {
                    var timeLeft = finishDate - DateTime.UtcNow;
                    if (timeLeft > TimeSpan.Zero)
                    {
                        PrintCommandIsOnCooldown((ICommandActor) actor, timeLeft);
                        return Task.FromResult(PermissionGrantResult.Deny);
                    }

                    permissionCooldown.Remove(permission);
                }
            }
            else
            {
                permissionCooldown = new Dictionary<string, DateTime>();
            }

            if (permissions.All(pm => !string.Equals(pm.Name?.Trim(), permission, StringComparison.OrdinalIgnoreCase)))
                return Task.FromResult(PermissionGrantResult.Default);

            var rocketPermission = permissions.Find(knownPermission =>
                string.Equals(knownPermission.Name?.Trim(), permission, StringComparison.OrdinalIgnoreCase));
            if (rocketPermission == null)
                return Task.FromResult(PermissionGrantResult.Default);

            permissionCooldown[permission] = DateTime.UtcNow.AddSeconds(rocketPermission.Cooldown);
            m_Cooldowns[actor.Id] = permissionCooldown;
            return Task.FromResult(PermissionGrantResult.Default);
        }

        private void PrintCommandIsOnCooldown(ICommandActor actor, TimeSpan timeLeft)
        {
            var message = m_StringLocalizer[
                "rocket:permissions:command_cooldown",
                new { TimeLeft = timeLeft.TotalSeconds, TimeLeftSpan = timeLeft }
            ];

            AsyncHelper.RunSync(() =>
                actor.PrintMessageAsync(message!, Color.Red)
            );
        }
    }
}
