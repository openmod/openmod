using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using OpenMod.API.Persistence;
using OpenMod.API.Prioritization;
using OpenMod.API.Users;
using OpenMod.Core.Helpers;

namespace OpenMod.Core.Users
{
    [UsedImplicitly]
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class UserDataStore : IUserDataStore
    {
        public const string UsersKey = "users";
        private readonly IDataStore m_DataStore;
        
        public UserDataStore(IOpenModDataStoreAccessor dataStoreAccessor)
        {
            m_DataStore = dataStoreAccessor.DataStore;
        }

        public async Task<UserData> GetUserDataAsync(string userId, string userType)
        {
            var usersData = await GetUsersDataAsync();
            return usersData.Users.FirstOrDefault(d => d.Type.Equals(userType, StringComparison.OrdinalIgnoreCase)
                                               && d.Id.Equals(userId, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<T> GetUserDataAsync<T>(string userId, string userType, string key)
        {
            var data = await GetUserDataAsync(userId, userType);
            if (!data.Data.ContainsKey(key))
            {
                return default;
            }

            var dataObject = data.Data[key];
            if (dataObject is T obj)
            {
                return obj;
            }

            if (dataObject.GetType().HasConversionOperator(typeof(T)))
            {
                // ReSharper disable once PossibleInvalidCastException
                return (T)dataObject;
            }

            if (dataObject is Dictionary<object, object> dict)
            {
                return dict.ToObject<T>();
            }

            throw new Exception($"Failed to parse {dataObject.GetType()} as {typeof(T)}");
        }

        public async Task<IReadOnlyCollection<UserData>> GetUsersDataAsync(string type)
        {
            var usersData = await GetUsersDataAsync();
            return usersData.Users.Where(d => d.Type.Equals(type, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public async Task SaveUserDataAsync(UserData userData)
        {
            var usersData = await GetUsersDataAsync();
            var idx = usersData.Users.FindIndex(c =>
                c.Type.Equals(userData.Type, StringComparison.OrdinalIgnoreCase) &&
                c.Id.Equals(userData.Id, StringComparison.OrdinalIgnoreCase));

            usersData.Users.RemoveAll(c =>
                c.Type.Equals(userData.Type, StringComparison.OrdinalIgnoreCase) &&
                c.Id.Equals(userData.Id, StringComparison.OrdinalIgnoreCase));

            // preserve location in data
            if (idx >= 0)
            {
                usersData.Users.Insert(idx, userData);
            }
            else
            {
                usersData.Users.Add(userData);
            }


            await m_DataStore.SaveAsync(UsersKey, usersData);
        }

        private async Task<UsersData> GetUsersDataAsync()
        {
            return await m_DataStore.LoadAsync<UsersData>(UsersKey) ?? new UsersData
            {
                Users = new List<UserData>()
            };
        }
    }
}
