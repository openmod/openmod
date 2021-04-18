using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.API.Permissions;
using OpenMod.API.Prioritization;
using OpenMod.API.Users;
using OpenMod.Core.Permissions.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenMod.Core.Permissions
{
    [OpenModInternal]
    [UsedImplicitly]
    [ServiceImplementation(Priority = Priority.Lowest, Lifetime = ServiceLifetime.Singleton)]
    public class DefaultPermissionRoleStore : IPermissionRoleStore
    {
        private readonly IPermissionRolesDataStore m_PermissionRolesDataStore;
        private readonly IUserDataStore m_UserDataStore;

        public DefaultPermissionRoleStore(IPermissionRolesDataStore permissionRolesDataStore, IUserDataStore userDataStore)
        {
            m_PermissionRolesDataStore = permissionRolesDataStore;
            m_UserDataStore = userDataStore;
        }

        public virtual async Task<IReadOnlyCollection<IPermissionRole>> GetRolesAsync(IPermissionActor actor, bool inherit = true)
        {
            if (actor == null)
            {
                throw new ArgumentNullException(nameof(actor));
            }

            var roles = new List<IPermissionRole>();
            var roleIds = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

            if (actor is IPermissionRole role)
            {
                roles.Add(role);
                roleIds.Add(role.Id);

                if (!inherit)
                    return roles;

                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (var parentRoleId in role.Parents)
                {
                    if (string.IsNullOrEmpty(parentRoleId)) continue;

                    if (roleIds.Contains(parentRoleId))
                        continue;

                    var parentRole = await GetRoleAsync(parentRoleId);
                    if (parentRole == null)
                        continue;

                    roles.Add(parentRole);
                    roleIds.Add(parentRoleId);
                }

                return roles;
            }

            var userData = await m_UserDataStore.GetUserDataAsync(actor.Id, actor.Type);
            if (userData?.Roles == null)
            {
                return GetAutoAssignRoles().ToList();
            }

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var roleId in userData.Roles)
            {
                if (string.IsNullOrEmpty(roleId)) continue;

                if (roleIds.Contains(roleId))
                {
                    continue;
                }

                var userRole = await GetRoleAsync(roleId);
                if (userRole == null)
                    continue;

                roles.Add(userRole);
                roleIds.Add(roleId);

                if (!inherit)
                    continue;

                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (var parentRoleId in userRole.Parents)
                {
                    if (string.IsNullOrEmpty(parentRoleId)) continue;

                    if (roleIds.Contains(parentRoleId))
                        continue;

                    var parentRole = await GetRoleAsync(parentRoleId);
                    if (parentRole == null)
                        continue;

                    roles.Add(parentRole);
                    roleIds.Add(parentRoleId);
                }
            }

            return roles;
        }

        public virtual Task<IReadOnlyCollection<IPermissionRole>> GetRolesAsync()
        {
            //cast is neccessary, OfType<> or Cast<> does not work with cast operators
            return Task.FromResult<IReadOnlyCollection<IPermissionRole>>(m_PermissionRolesDataStore.Roles
                .Where(d => d != null)
                .Select(d => (IPermissionRole)(PermissionRole)d)
                .ToList());
        }

        public virtual Task<IPermissionRole?> GetRoleAsync(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                throw new ArgumentNullException(nameof(roleId));
            }

            if (m_PermissionRolesDataStore.Roles.Count < 1)
            {
                return Task.FromResult<IPermissionRole?>(result: null);
            }

            //cast is neccessary, OfType<> or Cast<> does not work with cast operators
            return Task.FromResult<IPermissionRole?>(m_PermissionRolesDataStore.Roles
                .Select(d => (IPermissionRole)(PermissionRole)d)
                .FirstOrDefault(d => d.Id.Equals(roleId, StringComparison.OrdinalIgnoreCase)));
        }

        public virtual async Task<bool> UpdateRoleAsync(IPermissionRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            if (await GetRoleAsync(role.Id) == null)
            {
                return false;
            }

            var roleData = m_PermissionRolesDataStore.Roles.FirstOrDefault(d => d?.Id?.EndsWith(role.Id) ?? false);
            if (roleData == null)
            {
                throw new Exception($"Role does not exist: {role.Id}");
            }

            roleData.DisplayName = role.DisplayName;
            roleData.IsAutoAssigned = role.IsAutoAssigned;
            roleData.Parents = role.Parents;
            roleData.Permissions = role.Parents;
            roleData.Priority = role.Priority;
            roleData.Data ??= new Dictionary<string, object?>();

            await m_PermissionRolesDataStore.SaveChangesAsync();
            return true;
        }

        public virtual async Task<bool> AddRoleToActorAsync(IPermissionActor actor, string roleId)
        {
            if (actor == null)
            {
                throw new ArgumentNullException(nameof(actor));
            }

            if (string.IsNullOrEmpty(roleId))
            {
                throw new ArgumentNullException(nameof(roleId));
            }

            var userData = await m_UserDataStore.GetUserDataAsync(actor.Id, actor.Type);
            if (userData == null)
            {
                return false;
            }

            userData.Roles ??= new HashSet<string>();
            if (userData.Roles.Contains(roleId))
            {
                return true;
            }

            userData.Roles.Add(roleId);
            await m_UserDataStore.SetUserDataAsync(userData);
            return true;
        }

        public virtual async Task<bool> RemoveRoleFromActorAsync(IPermissionActor actor, string roleId)
        {
            if (actor == null)
            {
                throw new ArgumentNullException(nameof(actor));
            }

            if (string.IsNullOrEmpty(roleId))
            {
                throw new ArgumentNullException(nameof(roleId));
            }

            var userData = await m_UserDataStore.GetUserDataAsync(actor.Id, actor.Type);
            if (userData?.Roles == null)
            {
                return false;
            }

            userData.Roles.Remove(roleId);
            await m_UserDataStore.SetUserDataAsync(userData);
            return true;
        }

        public virtual async Task<bool> CreateRoleAsync(IPermissionRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            if (await GetRoleAsync(role.Id) != null)
            {
                return false;
            }

            m_PermissionRolesDataStore.Roles.Add(new PermissionRoleData
            {
                Priority = role.Priority,
                Id = role.Id,
                DisplayName = role.DisplayName,
                Parents = new HashSet<string>(role.Parents, StringComparer.InvariantCultureIgnoreCase),
                Permissions = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase),
                IsAutoAssigned = role.IsAutoAssigned
            });

            await m_PermissionRolesDataStore.SaveChangesAsync();
            return true;
        }

        public virtual async Task<bool> DeleteRoleAsync(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                throw new ArgumentNullException(nameof(roleId));
            }

            if (m_PermissionRolesDataStore.Roles.RemoveAll(c => roleId.Equals(c.Id, StringComparison.OrdinalIgnoreCase)) == 0)
            {
                return false;
            }

            await m_PermissionRolesDataStore.SaveChangesAsync();
            return true;
        }

        public Task<IReadOnlyCollection<string>> GetAutoAssignedRolesAsync(string actorId, string actorType)
        {
            if (string.IsNullOrEmpty(actorId))
            {
                throw new ArgumentNullException(nameof(actorId));
            }

            if (string.IsNullOrEmpty(actorType))
            {
                throw new ArgumentNullException(nameof(actorType));
            }

            IReadOnlyCollection<string> result = GetAutoAssignRoles().Select(d => d.Id).ToList();
            return Task.FromResult(result);
        }

        public async Task SavePersistentDataAsync<T>(string roleId, string key, T? data)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                throw new ArgumentNullException(nameof(roleId));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            var roleData = m_PermissionRolesDataStore.Roles.FirstOrDefault(d => d.Id?.Equals(roleId, StringComparison.OrdinalIgnoreCase) ?? false);
            if (roleData == null)
            {
                throw new Exception($"Role does not exist: {roleId}");
            }

            if(roleData.Data == null)
            {
                roleData.Data = new Dictionary<string, object?>();
            }

            if (roleData.Data.ContainsKey(key))
            {
                roleData.Data[key] = data;
            }
            else
            {
                roleData.Data.Add(key, data);
            }

            await m_PermissionRolesDataStore.SaveChangesAsync();
        }

        public Task<T?> GetPersistentDataAsync<T>(string roleId, string key)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                throw new ArgumentNullException(nameof(roleId));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            return m_PermissionRolesDataStore.GetRoleDataAsync<T>(roleId, key);
        }

        protected IEnumerable<IPermissionRole> GetAutoAssignRoles()
        {
            //cast is neccessary, OfType<> or Cast<> does not work with cast operators
            return m_PermissionRolesDataStore.Roles
                .Select(d => (PermissionRole)d)
                .Where(d => d.IsAutoAssigned);
        }
    }
}