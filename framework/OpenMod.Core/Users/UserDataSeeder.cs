using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using OpenMod.API.Permissions;
using OpenMod.API.Prioritization;
using OpenMod.API.Users;

namespace OpenMod.Core.Users
{
    [UsedImplicitly]
    [ServiceImplementation(Priority = Priority.Lowest, Lifetime = ServiceLifetime.Singleton)]
    public class UserDataSeeder : IUserDataSeeder
    {
        private readonly IUserDataStore m_UserDataStore;
        private readonly IPermissionRoleStore m_PermissionRoleStore;

        public UserDataSeeder(IUserDataStore userDataStore, IPermissionRoleStore permissionRoleStore)
        {
            m_UserDataStore = userDataStore;
            m_PermissionRoleStore = permissionRoleStore;
        }

        public async Task SeedUserDataAsync(string actorId, string actorType, string displayName, Dictionary<string, object> data = null)
        {
            var userData = await m_UserDataStore.GetUserDataAsync(actorId, actorType);
            if (userData != null)
            {
                return; // no seeding
            }

            var autoAssignRoles = new HashSet<string>(await m_PermissionRoleStore.GetAutoAssignedRolesAsync(actorId, actorType));

            userData = new UserData
            {
                Data = data ?? new Dictionary<string, object>(),
                Id = actorId,
                Type = actorType,
                LastSeen = DateTime.Now,
                FirstSeen = DateTime.Now,
                Permissions = new HashSet<string>(),
                Roles = autoAssignRoles,
                LastDisplayName = displayName
            };

            await m_UserDataStore.SaveUserDataAsync(userData);
        }
    }
}