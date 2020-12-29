using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.API.Persistence;
using OpenMod.API.Prioritization;
using OpenMod.API.Users;
using OpenMod.Common.Helpers;
using OpenMod.Core.Helpers;

namespace OpenMod.Core.Users
{
    [UsedImplicitly]
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class UserDataStore : IUserDataStore, IAsyncDisposable
    {
        private readonly ILogger<UserDataStore> m_Logger;
        private readonly IRuntime m_Runtime;
        public const string UsersKey = "users";
        private readonly IDataStore m_DataStore;
        private IDisposable m_FileChangeWatcher;
        private UsersData m_CachedUsersData;
        private bool m_IsUpdating;

        public UserDataStore(
            ILogger<UserDataStore> logger,
            IOpenModDataStoreAccessor dataStoreAccessor,
            IRuntime runtime)
        {
            m_Logger = logger;
            m_Runtime = runtime;
            m_DataStore = dataStoreAccessor.DataStore;

            AsyncHelper.RunSync(async () =>
            {
                m_CachedUsersData = await LoadUsersDataFromDiskAsync();
            });
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

        public async Task SetUserDataAsync<T>(string userId, string userType, string key, T value)
        {
            var userData = await GetUserDataAsync(userId, userType);
            if (userData.Data == null)
            {
                userData.Data = new Dictionary<string, object>();
            }
            else if (userData.Data.ContainsKey(key))
            {
                userData.Data.Remove(key);
            }

            userData.Data.Add(key, value);
            await SetUserDataAsync(userData);
        }

        public async Task<IReadOnlyCollection<UserData>> GetUsersDataAsync(string type)
        {
            var usersData = await GetUsersDataAsync();
            return usersData.Users.Where(d => d.Type.Equals(type, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public async Task SetUserDataAsync(UserData userData)
        {
            m_Logger.LogDebug("SetUserDataAsync for user: " + userData.Id + "\n" + new StackTrace());

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

            m_CachedUsersData = usersData;

            m_IsUpdating = true;
            await m_DataStore.SaveAsync(UsersKey, m_CachedUsersData);
        }

        private async Task<UsersData> GetUsersDataAsync()
        {
            if (m_CachedUsersData != null)
            {
                return m_CachedUsersData;
            }

            var created = false;
            if (!await m_DataStore.ExistsAsync(UsersKey))
            {
                m_CachedUsersData = new UsersData { Users = new List<UserData>() };

                await m_DataStore.SaveAsync(UsersKey, m_CachedUsersData);
                created = true;
            }

            m_FileChangeWatcher = m_DataStore.AddChangeWatcher(UsersKey, m_Runtime, () =>
            {
                if (!m_IsUpdating)
                {
                    m_CachedUsersData = AsyncHelper.RunSync(LoadUsersDataFromDiskAsync);
                }

                m_IsUpdating = false;
            });

            if (!created)
            {
                m_CachedUsersData = await LoadUsersDataFromDiskAsync();
            }
            return m_CachedUsersData;
        }

        private async Task<UsersData> LoadUsersDataFromDiskAsync()
        {
            if (!await m_DataStore.ExistsAsync(UsersKey))
            {
                m_CachedUsersData = new UsersData
                {
                    Users = new List<UserData>()
                };

                await m_DataStore.SaveAsync(UsersKey, m_CachedUsersData);
                return m_CachedUsersData;
            }

            return await m_DataStore.LoadAsync<UsersData>(UsersKey) ?? new UsersData
            {
                Users = new List<UserData>()
            };
        }

        public async ValueTask DisposeAsync()
        {
            m_FileChangeWatcher?.Dispose();

            if (m_CachedUsersData?.Users == null)
            {
                throw new Exception("Tried to save null users data");
            }

            await m_DataStore.SaveAsync(UsersKey, m_CachedUsersData);
        }
    }
}
