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
        private readonly IPermissionGroupStore m_PermissionGroupStore;

        public UserDataSeeder(IUserDataStore userDataStore, IPermissionGroupStore permissionGroupStore)
        {
            m_UserDataStore = userDataStore;
            m_PermissionGroupStore = permissionGroupStore;
        }

        public async Task SeedUserDataAsync(string actorId, string actorType, string displayName, Dictionary<string, object> data = null)
        {
            var userData = await m_UserDataStore.GetUserDataAsync(actorId, actorType);
            if (userData != null)
            {
                return; // no seeding
            }

            var autoAssignGroups = new HashSet<string>(await m_PermissionGroupStore.GetAssignAutoGroupsAsync(actorId, actorType));

            userData = new UserData
            {
                Data = data ?? new Dictionary<string, object>(),
                Id = actorId,
                Type = actorType,
                LastSeen = DateTime.Now,
                FirstSeen = DateTime.Now,
                Permissions = new HashSet<string>(),
                Groups = autoAssignGroups,
                LastDisplayName = displayName
            };

            await m_UserDataStore.SaveUserDataAsync(userData);
        }
    }
}