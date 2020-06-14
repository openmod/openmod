using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using OpenMod.API.Persistence;
using OpenMod.API.Prioritization;
using OpenMod.Core.Helpers;
using OpenMod.Core.Permissions.Data;

namespace OpenMod.Core.Users
{
    [UsedImplicitly]
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class UsersDataStore : IUsersDataStore
    {
        public const string UsersKey = "users";
        public List<UserData> Users { get; private set; }
        
        private readonly IDataStore m_DataStore;
        
        public UsersDataStore(IOpenModDataStoreAccessor dataStoreAccessor)
        {
            m_DataStore = dataStoreAccessor.DataStore;
            AsyncHelper.RunSync(InitAsync);
        }

        private async Task InitAsync()
        {
            if (!await ExistsAsync())
            {
                Users = new List<UserData>();
                await SaveChangesAsync();
            }
            else
            {
                await ReloadAsync();
            }
        }

        public async Task ReloadAsync()
        {
            Users = (await m_DataStore.LoadAsync<UsersData>(UsersKey))?.Users ?? new List<UserData>();
        }

        public Task SaveChangesAsync()
        {
            return m_DataStore.SaveAsync(UsersKey, new UsersData {  Users = Users });
        }

        public Task<bool> ExistsAsync()
        {
            return m_DataStore.ExistsAsync(UsersKey);
        }

        public Task<UserData> GetUserAsync(string userType, string userId)
        {
            var user = Users.FirstOrDefault(d => d.Type.Equals(userType, StringComparison.OrdinalIgnoreCase) 
                                                 && d.Id.Equals(userId, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(user);
        }
    }
}
