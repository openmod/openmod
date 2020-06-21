using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenMod.API.Permissions;
using OpenMod.API.Prioritization;
using OpenMod.API.Users;

namespace OpenMod.Core.Permissions
{
    [Priority(Priority = Priority.Lowest)]
    public class DefaultPermissionStore : IPermissionStore
    {
        private readonly IUserDataStore m_UserDataStore;
        private readonly IPermissionRolesDataStore m_PermissionRolesDataStore;
        private readonly IPermissionRoleStore m_PermissionRoleStore;

        public DefaultPermissionStore(
            IUserDataStore userDataStore,
            IPermissionRolesDataStore permissionRolesDataStore,
            IPermissionRoleStore permissionRoleStore)
        {
            m_UserDataStore = userDataStore;
            m_PermissionRolesDataStore = permissionRolesDataStore;
            m_PermissionRoleStore = permissionRoleStore;
        }
        public virtual Task<IReadOnlyCollection<string>> GetGrantedPermissionsAsync(IPermissionActor actor, bool inherit = true)
        {
            return GetGrantDenyPermissionsAsync(actor, inherit, true);
        }

        public virtual Task<IReadOnlyCollection<string>> GetDeniedPermissionsAsync(IPermissionActor actor, bool inherit = true)
        {
            return GetGrantDenyPermissionsAsync(actor, inherit, false);
        }

        protected virtual async Task<IReadOnlyCollection<string>> GetGrantDenyPermissionsAsync(IPermissionActor actor, bool inherit, bool isGrant)
        {
            var deniedPerms = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            var grantedPerms = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var permission in await GetPermissionsAsync(actor, inherit)) //Linq is Ugly
            {
                var isDeny = permission.StartsWith("!");
                var trimmedPermission = permission.TrimStart('!');

                if (grantedPerms.Contains(trimmedPermission) || deniedPerms.Contains(trimmedPermission)) //if already added or if is denied by priority
                    continue;

                if (isDeny)
                {
                    deniedPerms.Add(trimmedPermission);
                }
                else
                {
                    grantedPerms.Add(trimmedPermission);
                }
            }

            return isGrant ? grantedPerms : deniedPerms;
        }

        protected async Task<HashSet<string>> GetPermissionsAsync(IPermissionActor actor, bool inherit = true) //order by descending priority
        {
            var permissions = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            var user = await m_UserDataStore.GetUserDataAsync(actor.Id, actor.Type);
            if (user != null)
            {
                permissions.UnionWith(user.Permissions);
            }

            foreach (var role in (await m_PermissionRoleStore.GetRolesAsync(actor, inherit)).OrderByDescending(gp => gp.Priority))
            {
                permissions.UnionWith(((PermissionRole)role).Permissions);
            }

            return permissions;
        }

        public virtual async Task<bool> AddGrantedPermissionAsync(IPermissionActor actor, string permission)
        {
            if (actor is IPermissionRole)
            {
                var roleData = m_PermissionRolesDataStore.Roles.First(d => d.Id.Equals(actor.Id, StringComparison.OrdinalIgnoreCase));
                if (roleData == null)
                {
                    return false;
                }

                roleData.Permissions.Add(permission);
                await m_PermissionRolesDataStore.SaveChangesAsync();
                return true;
            }

            var userData = await m_UserDataStore.GetUserDataAsync(actor.Id, actor.Type);
            userData.Permissions.Add(permission);
            await m_UserDataStore.SaveUserDataAsync(userData);
            return true;
        }

        public virtual Task<bool> AddDeniedPermissionAsync(IPermissionActor actor, string permission)
        {
            return AddGrantedPermissionAsync(actor, "!" + permission);
        }

        public virtual async Task<bool> RemoveGrantedPermissionAsync(IPermissionActor actor, string permission)
        {
            if (actor is IPermissionRole)
            {
                var roleData = m_PermissionRolesDataStore.Roles.First(d => d.Id.Equals(actor.Id, StringComparison.OrdinalIgnoreCase));
                if (roleData == null)
                {
                    return false;
                }

                if (!roleData.Permissions.Remove(permission)) 
                    return false;

                await m_PermissionRolesDataStore.SaveChangesAsync();
                return true;
            }

            var userData = await m_UserDataStore.GetUserDataAsync(actor.Id, actor.Type);
            if (!userData.Permissions.Remove(permission))
                return false;

            await m_UserDataStore.SaveUserDataAsync(userData);
            return true;
        }

        public virtual Task<bool> RemoveDeniedPermissionAsync(IPermissionActor actor, string permission)
        {
            return RemoveGrantedPermissionAsync(actor, "!" + permission);
        }
    }
}