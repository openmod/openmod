using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenMod.API.Permissions;
using OpenMod.Core.Users;
using Rocket.API;
using Rocket.API.Serialisation;
using Rocket.Core;

namespace OpenMod.Unturned.RocketMod.Permissions
{
    public class RocketPermissionStore : IPermissionStore
    {
        public Task<IReadOnlyCollection<string>> GetGrantedPermissionsAsync(IPermissionActor actor, bool inherit = true)
        {
            if (!RocketModIntegrationEnabled())
            {
                return Task.FromResult<IReadOnlyCollection<string>>(new List<string>());
            }

            return Task.FromResult<IReadOnlyCollection<string>>(GetRocketModPermissions(actor, inherit, isDenied: false));
        }

        public Task<IReadOnlyCollection<string>> GetDeniedPermissionsAsync(IPermissionActor actor, bool inherit = true)
        {
            if (!RocketModIntegrationEnabled() || !IsPlayerActor(actor.Type))
            {
                return Task.FromResult<IReadOnlyCollection<string>>(new List<string>());
            }

            return Task.FromResult<IReadOnlyCollection<string>>(GetRocketModPermissions(actor, inherit, isDenied: true));
        }

        public Task<bool> AddGrantedPermissionAsync(IPermissionActor actor, string permission)
        {
            if (!(actor is IPermissionRole role))
            {
                return Task.FromException<bool>(new NotSupportedException("Cannot add permissions to RocketMod players."));
            }

            return Task.FromResult(UpdateGroupPermission(role, permission, add: true));
        }

        public Task<bool> AddDeniedPermissionAsync(IPermissionActor actor, string permission)
        {
            if (!(actor is IPermissionRole role))
            {
                return Task.FromException<bool>(new NotSupportedException("Cannot add permissions to RocketMod players."));
            }

            return Task.FromResult(UpdateGroupPermission(role, $"!{permission}", add: true));
        }

        public Task<bool> RemoveGrantedPermissionAsync(IPermissionActor actor, string permission)
        {
            if (!(actor is IPermissionRole role))
            {
                return Task.FromException<bool>(new NotSupportedException("Cannot remove permissions from RocketMod players."));
            }

            return Task.FromResult(UpdateGroupPermission(role, permission, add: false));
        }

        public Task<bool> RemoveDeniedPermissionAsync(IPermissionActor actor, string permission)
        {
            if (!(actor is IPermissionRole role))
            {
                return Task.FromException<bool>(new NotSupportedException("Cannot remove permissions from RocketMod players."));
            }

            return Task.FromResult(UpdateGroupPermission(role, $"!{permission}", add: false));
        }

        private bool RocketModIntegrationEnabled()
        {
            // todo: check from config
            return RocketModIntegration.IsRocketModReady();
        }

        private RocketPlayer ToRocketPlayer(IPermissionActor actor)
        {
            return new RocketPlayer(actor.Id, actor.DisplayName);
        }

        private bool UpdateGroupPermission(IPermissionRole role, string permission, bool add)
        {
            var group = R.Permissions.GetGroup(role.Id);
            if (group == null)
            {
                return false;
            }

            if (add)
            {
                group.Permissions.Add(new Permission(permission));
            }
            else
            {
                group.Permissions.Remove(new Permission(permission));
            }

            R.Permissions.SaveGroup(group);
            return true;
        }

        private bool IsPlayerActor(string actorType)
        {
            return actorType.Equals(KnownActorTypes.Player, StringComparison.OrdinalIgnoreCase);
        }

        private List<string> GetRocketModPermissions(IPermissionActor actor, bool inherit, bool isDenied)
        {
            if (IsPlayerActor(actor.Type))
            {
                if (!inherit)
                {
                    // RocketMod does not support adding permissions to players directly
                    return new List<string>();
                }

                return
                    R.Permissions.GetPermissions(ToRocketPlayer(actor))
                        .Where(d => isDenied ? d.Name.StartsWith("!") : !d.Name.StartsWith("!"))
                        .Select(d => d.Name)
                        .ToList();
            }

            var list = new List<string>();
            var currentGroup = R.Permissions.GetGroup(actor.Id);

            // traverse parent groups
            while (currentGroup != null)
            {
                if (currentGroup.Permissions != null)
                {
                    list.AddRange(currentGroup.Permissions.Select(d => d.Name));
                }

                if (!inherit)
                {
                    currentGroup = null;
                    continue;
                }

                currentGroup = R.Permissions.GetGroup(currentGroup.ParentGroup);
            }

            return list
                .Where(d => isDenied ? d.StartsWith("!") : !d.StartsWith("!"))
                .ToList();
        }
    }
}