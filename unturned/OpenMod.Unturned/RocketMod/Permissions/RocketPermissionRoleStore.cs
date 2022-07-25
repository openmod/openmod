using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.API.Permissions;
using OpenMod.Core.Ioc;
using OpenMod.Core.Users;
using Rocket.API;
using Rocket.Core;

namespace OpenMod.Unturned.RocketMod.Permissions
{
    [DontAutoRegister]
    public class RocketPermissionRoleStore : IPermissionRoleStore
    {
        private readonly IPermissionRoleStore m_BasePermissionRoleStore;

        public RocketPermissionRoleStore(IPermissionRoleStore basePermissionRoleStore)
        {
            m_BasePermissionRoleStore = basePermissionRoleStore;
        }

        public async Task<IReadOnlyCollection<IPermissionRole>> GetRolesAsync(IPermissionActor actor, bool inherit = true)
        {
            if (actor == null)
            {
                throw new ArgumentNullException(nameof(actor));
            }

            var list = new List<IPermissionRole>();
            list.AddRange(await m_BasePermissionRoleStore.GetRolesAsync(actor, inherit));

            if (RocketModIntegrationEnabled() && IsPlayerActor(actor.Type))
            {
                foreach (var group in R.Permissions.GetGroups(ToRocketPlayer(actor), inherit))
                {
                    list.Add(new RocketGroupWrapper(group));
                }
            }

            return list;
        }

        public async Task<IReadOnlyCollection<IPermissionRole>> GetRolesAsync()
        {
            var list = new List<IPermissionRole>();
            list.AddRange(await m_BasePermissionRoleStore.GetRolesAsync());

            if (RocketModIntegrationEnabled())
            {
                // todo: add RocketMod groups
                // however RocketMod does not provide an API for this
                // would not support plugins like Advanced Permissions
            }

            return list;
        }

        public async Task<IPermissionRole?> GetRoleAsync(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                throw new ArgumentException(nameof(roleId));
            }

            if (RocketModIntegrationEnabled() && IsRocketModRole(roleId))
            {
                return new RocketGroupWrapper(R.Permissions.GetGroup(roleId));
            }

            return await m_BasePermissionRoleStore.GetRoleAsync(roleId);
        }

        public Task<bool> UpdateRoleAsync(IPermissionRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            if (RocketModIntegrationEnabled() && IsRocketModRole(role.Id))
            {
                Task.FromException(new NotSupportedException("Updating RocketMod roles from OpenMod is not supported."));
            }

            return m_BasePermissionRoleStore.UpdateRoleAsync(role);
        }

        public Task<bool> AddRoleToActorAsync(IPermissionActor actor, string roleId)
        {
            if (actor == null)
            {
                throw new ArgumentNullException(nameof(actor));
            }

            if (string.IsNullOrEmpty(roleId))
            {
                throw new ArgumentException(nameof(roleId));
            }

            if (RocketModIntegrationEnabled() && IsRocketModRole(roleId))
            {
                if (!IsPlayerActor(actor.Type))
                {
                    return Task.FromException<bool>(new NotSupportedException($"Cannot add non-player actor {actor.Type} to a RocketMod group."));
                }

                var result = R.Permissions.AddPlayerToGroup(roleId, ToRocketPlayer(actor));
                return Task.FromResult(result == RocketPermissionsProviderResult.Success);
            }

            return m_BasePermissionRoleStore.AddRoleToActorAsync(actor, roleId);
        }

        public Task<bool> RemoveRoleFromActorAsync(IPermissionActor actor, string roleId)
        {
            if (actor == null)
            {
                throw new ArgumentNullException(nameof(actor));
            }

            if (string.IsNullOrEmpty(roleId))
            {
                throw new ArgumentException(nameof(roleId));
            }

            if (RocketModIntegrationEnabled() && IsRocketModRole(roleId))
            {
                if (!IsPlayerActor(actor.Type))
                {
                    return Task.FromException<bool>(new NotSupportedException($"Cannot remove non-player actor {actor.Type} from a RocketMod group."));
                }

                var result = R.Permissions.RemovePlayerFromGroup(roleId, ToRocketPlayer(actor));
                return Task.FromResult(result == RocketPermissionsProviderResult.Success);
            }

            return m_BasePermissionRoleStore.RemoveRoleFromActorAsync(actor, roleId);
        }

        public Task<bool> CreateRoleAsync(IPermissionRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return m_BasePermissionRoleStore.CreateRoleAsync(role);
        }

        public Task<bool> DeleteRoleAsync(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                throw new ArgumentException(nameof(roleId));
            }

            if (RocketModIntegrationEnabled() && IsRocketModRole(roleId))
            {
                var result = R.Permissions.DeleteGroup(roleId);
                return Task.FromResult(result == RocketPermissionsProviderResult.Success);
            }

            return m_BasePermissionRoleStore.DeleteRoleAsync(roleId);
        }

        public async Task<IReadOnlyCollection<string>> GetAutoAssignedRolesAsync(string actorId, string actorType)
        {
            if (string.IsNullOrEmpty(actorId))
            {
                throw new ArgumentException(nameof(actorId));
            }

            if (string.IsNullOrEmpty(actorType))
            {
                throw new ArgumentException(nameof(actorType));
            }

            var list = new List<string>();
            list.AddRange(await m_BasePermissionRoleStore.GetAutoAssignedRolesAsync(actorId, actorType));

            if (RocketModIntegrationEnabled())
            {
                // todo: add default RocketMod group
            }

            return list;
        }

        public Task SavePersistentDataAsync<T>(string roleId, string key, T? data)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                throw new ArgumentException(nameof(roleId));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException(nameof(key));
            }

            if (RocketModIntegrationEnabled() && IsRocketModRole(roleId))
            {
                return Task.FromException(new NotSupportedException("Persistent data is not supported for RocketMod roles."));
            }

            return m_BasePermissionRoleStore.SavePersistentDataAsync(roleId, key, data);
        }

        public Task<T?> GetPersistentDataAsync<T>(string roleId, string key)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                throw new ArgumentException(nameof(roleId));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException(nameof(key));
            }

            if (RocketModIntegrationEnabled() && IsRocketModRole(roleId))
            {
                return Task.FromException<T?>(new NotSupportedException("Persistent data is not supported for RocketMod roles."));
            }

            return m_BasePermissionRoleStore.GetPersistentDataAsync<T>(roleId, key);
        }

        private bool RocketModIntegrationEnabled()
        {
            // todo: check from config
            return RocketModIntegration.IsRocketModReady();
        }

        private RocketPlayer ToRocketPlayer(IPermissionActor actor)
        {
            return new(actor.Id, actor.DisplayName);
        }

        private bool IsPlayerActor(string actorType)
        {
            return actorType.Equals(KnownActorTypes.Player, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsRocketModRole(string id)
        {
            if (!RocketModIntegration.IsRocketModReady())
            {
                return false;
            }

            return R.Permissions.GetGroup(id) != null;
        }
    }
}