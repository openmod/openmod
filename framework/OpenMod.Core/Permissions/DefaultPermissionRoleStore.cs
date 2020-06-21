using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using OpenMod.API.Permissions;
using OpenMod.API.Prioritization;
using OpenMod.API.Users;
using OpenMod.Core.Helpers;
using OpenMod.Core.Permissions.Data;
using OpenMod.Core.Users;

namespace OpenMod.Core.Permissions
{
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
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var roleId in userData.Roles)
            {
                if (roleIds.Contains(roleId)) //prevent add roles that was already by parent for example
                    continue;

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
            return Task.FromResult((IReadOnlyCollection<IPermissionRole>)m_PermissionRolesDataStore.Roles
                .Select(d => (IPermissionRole)(PermissionRole)d)
                .ToList());
        }

        public virtual Task<IPermissionRole> GetRoleAsync(string id)
        {
            //cast is neccessary, OfType<> or Cast<> does not work with cast operators
            return Task.FromResult(m_PermissionRolesDataStore.Roles
                .Select(d => (IPermissionRole)(PermissionRole)d)
                .FirstOrDefault(d => d.Id.Equals(id, StringComparison.OrdinalIgnoreCase)));
        }

        public virtual async Task<bool> UpdateRoleAsync(IPermissionRole role)
        {
            if (await GetRoleAsync(role.Id) == null)
            {
                return false;
            }

            var roleData = m_PermissionRolesDataStore.Roles.First(d => d.Id.EndsWith(role.Id));
            roleData.DisplayName = role.DisplayName;
            roleData.IsAutoAssigned = role.IsAutoAssigned;
            roleData.Parents = role.Parents;
            roleData.Permissions = role.Parents;
            roleData.Priority = role.Priority;
            roleData.Data ??= new Dictionary<string, object>();

            await m_PermissionRolesDataStore.SaveChangesAsync();
            return true;
        }

        public virtual async Task<bool> AddRoleToActorAsync(IPermissionActor actor, string roleId)
        {
            var userData = await m_UserDataStore.GetUserDataAsync(actor.Id, actor.Type);
            if (userData.Roles.Contains(roleId))
            {
                return true;
            }

            userData.Roles.Add(roleId);
            await m_UserDataStore.SaveUserDataAsync(userData);
            return true;
        }

        public virtual async Task<bool> RemoveRoleFromActorAsync(IPermissionActor actor, string roleId)
        {
            var userData = await m_UserDataStore.GetUserDataAsync(actor.Id, actor.Type);
            userData.Roles.Remove(roleId);
            await m_UserDataStore.SaveUserDataAsync(userData);
            return true;
        }

        public virtual async Task<bool> CreateRoleAsync(IPermissionRole role)
        {
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
            if (m_PermissionRolesDataStore.Roles.RemoveAll(c => roleId.Equals(c.Id, StringComparison.OrdinalIgnoreCase)) == 0)
            {
                return false;
            }

            await m_PermissionRolesDataStore.SaveChangesAsync();
            return true;
        }

        public Task<IReadOnlyCollection<string>> GetAutoAssignedRolesAsync(string actorId, string actorType)
        {
            IReadOnlyCollection<string> result = GetAutoAssignRoles().Select(d => d.Id).ToList();
            return Task.FromResult(result);
        }

        public async Task SavePersistentDataAsync<T>(string roleId, string key, T data) where T : class
        {
            var roleData = m_PermissionRolesDataStore.Roles.FirstOrDefault(d => d.Id.Equals(roleId, StringComparison.OrdinalIgnoreCase));
            if (roleData == null)
            {
                throw new Exception($"Role does not exist: {roleId}");
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

        public Task<T> GetPersistentDataAsync<T>(string roleId, string key) where T : class
        {
            var roleData = m_PermissionRolesDataStore.Roles.FirstOrDefault(d => d.Id.Equals(roleId, StringComparison.OrdinalIgnoreCase));
            if (roleData == null)
            {
                return Task.FromException<T>(new Exception($"Role does not exist: {roleId}"));
            }

            if (!roleData.Data.ContainsKey(key))
            {
                return Task.FromResult<T>(null);
            }

            var dataObject = roleData.Data[key];

            if (dataObject is T obj)
            {
                return Task.FromResult(obj);
            }

            if (dataObject.GetType().HasConversionOperator(typeof(T)))
            {
                // ReSharper disable once PossibleInvalidCastException
                return Task.FromResult((T)dataObject);
            }

            if (dataObject is Dictionary<string, object> dict)
            {
                return Task.FromResult(dict.ToObject<T>());
            }

            throw new Exception($"Failed to parse {dataObject.GetType()} as {typeof(T)}");
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