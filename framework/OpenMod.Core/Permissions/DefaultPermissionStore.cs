using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenMod.API.Permissions;
using OpenMod.API.Prioritization;
using OpenMod.Core.Helpers;
using OpenMod.Core.Permissions.Data;

namespace OpenMod.Core.Permissions
{
    [Priority(Priority = Priority.Lowest)]
    public class DefaultPermissionStore : IPermissionStore
    {
        private readonly IPermissionFileManager m_PermissionFileManager;
        private readonly IPermissionGroupStore m_PermissionGroupStore;

        public DefaultPermissionStore(IPermissionFileManager permissionFileManager, IPermissionGroupStore permissionGroupStore)
        {
            m_PermissionFileManager = permissionFileManager;
            m_PermissionGroupStore = permissionGroupStore;
        }
        public virtual async Task<IReadOnlyCollection<string>> GetGrantedPermissionsAsync(IPermissionActor actor, bool inherit = true)
        {
            return (await GetPermissions(actor, inherit))
                .Where(d => !d.StartsWith("!"))
                .ToList();
        }

        public virtual async Task<IReadOnlyCollection<string>> GetDeniedPermissionsAsync(IPermissionActor actor, bool inherit = true)
        {
            return (await GetPermissions(actor, inherit))
                .Where(d => d.StartsWith("!"))
                .ToList();
        }

        protected async Task<List<string>> GetPermissions(IPermissionActor actor, bool inherit = true)
        {
            var permissions = new List<string>();
            if (actor is IPermissionGroup permissionGroup)
            {
                permissions.AddRange(((PermissionGroup)permissionGroup).Permissions);
            }
            else
            {
                var user = await GetOrCreateUserDataAsync(actor);
                permissions.AddRange(user.Permissions);
            }

            if (inherit)
            {
                foreach (var parent in await m_PermissionGroupStore.GetGroupsAsync(actor))
                {
                    permissions.AddRange(await GetPermissions(parent));
                }
            }

            return permissions;
        }

        public virtual async Task<bool> AddPermissionAsync(IPermissionActor actor, string permission)
        {
            if (actor is IPermissionGroup)
            {
                var groupData = m_PermissionFileManager.PermissionGroupsData.PermissionGroups.First(d => d.Id.Equals(actor.Id, StringComparison.OrdinalIgnoreCase));
                if (groupData == null)
                {
                    return false;
                }

                groupData.Permissions.Add(permission);
                await m_PermissionFileManager.SavePermissionGroupsAsync();
                return true;
            }

            var user = await GetOrCreateUserDataAsync(actor);
            user.Permissions.Add(permission);
            await m_PermissionFileManager.SaveUsersAsync();
            return true;
        }

        public virtual Task<bool> AddDeniedPermissionAsync(IPermissionActor actor, string permission)
        {
            return AddPermissionAsync(actor, "!" + permission);
        }

        public virtual async Task<bool> RemovePermissionAsync(IPermissionActor actor, string permission)
        {
            bool result;
            if (actor is IPermissionGroup)
            {
                var groupData = m_PermissionFileManager.PermissionGroupsData.PermissionGroups.First(d => d.Id.Equals(actor.Id, StringComparison.OrdinalIgnoreCase));
                if (groupData == null)
                {
                    return false;
                }

                result = groupData.Permissions.RemoveAll(c => c.Equals(permission, StringComparison.OrdinalIgnoreCase)) > 0;
                if (result)
                {
                    await m_PermissionFileManager.SavePermissionGroupsAsync();
                }
                
                return result;
            }

            var user = await GetOrCreateUserDataAsync(actor);
            result = user.Permissions.RemoveAll(c => c.Equals(permission, StringComparison.OrdinalIgnoreCase)) > 0;

            if (result)
            {
                await m_PermissionFileManager.SaveUsersAsync();
            }
            return result;
        }

        public virtual Task<bool> RemoveDeniedPermissionAsync(IPermissionActor actor, string permission)
        {
            return RemovePermissionAsync(actor, "!" + permission);
        }

        protected virtual async Task<UserData> GetOrCreateUserDataAsync(IPermissionActor actor)
        {
            var user = m_PermissionFileManager.UsersData.Users.FirstOrDefault(d => d.Id == actor.Id);
            if (user != null)
            {
                return user;
            }

            var userData = new UserData
            {
                Id = actor.Id,
                Type = actor.Type,
                FirstSeen = DateTime.Now,
                LastDisplayName = actor.DisplayName,
                LastSeen = DateTime.Now,
                Data = new Dictionary<string, object>(),
                Permissions = new List<string>(),
            };

            m_PermissionFileManager.UsersData.Users.Add(userData);
            await m_PermissionFileManager.SaveUsersAsync();
            await m_PermissionGroupStore.AssignAutoGroupsToUserAsync(actor);
            return userData;
        }
    }
}