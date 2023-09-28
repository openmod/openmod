using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.API.Persistence;
using OpenMod.API.Prioritization;
using OpenMod.API.Users;
using OpenMod.Core.Helpers;
using OpenMod.Core.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenMod.Core.Configuration;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace OpenMod.Core.Users
{
    [UsedImplicitly]
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class UserDataStore : IUserDataStore, IAsyncDisposable
    {
        private readonly IRuntime m_Runtime;
        private readonly IDataStore m_DataStore;
        private UsersData m_CachedUsersData;
        private IDisposable m_FileChangeWatcher;
        private bool m_IsUpdating;

        private readonly ISerializer m_Serializer;
        private readonly IDeserializer m_Deserializer;

        public const string UsersKey = "users";

        public UserDataStore(
            IOpenModDataStoreAccessor dataStoreAccessor,
            IRuntime runtime)
        {
            m_Runtime = runtime;
            m_DataStore = dataStoreAccessor.DataStore;

            m_Serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeConverter(new YamlNullableEnumTypeConverter())
                .DisableAliases()
                .Build();

            m_Deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .WithTypeConverter(new YamlNullableEnumTypeConverter())
                .Build();

            // suppress errors because the compiler can't analyze that the values are set from the statements below
            m_CachedUsersData = null!;
            m_FileChangeWatcher = null!;

            AsyncHelper.RunSync(async () => { m_CachedUsersData = await EnsureUserDataCreatedAsync(); });
        }

        private async Task<UsersData> EnsureUserDataCreatedAsync()
        {
            var created = false;
            if (!await m_DataStore.ExistsAsync(UsersKey))
            {
                m_CachedUsersData = new UsersData { Users = GetDefaultUsersData() };

                await SaveAsync();
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

        public async Task<UserData?> GetUserDataAsync(string userId, string userType)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException(nameof(userId));
            }

            if (string.IsNullOrEmpty(userType))
            {
                throw new ArgumentException(nameof(userType));
            }

            var usersData = await GetUsersDataAsync();
            return usersData?.FirstOrDefault(d =>
                (d?.Type?.Equals(userType, StringComparison.OrdinalIgnoreCase) ?? false)
                && (d.Id?.Equals(userId, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        public async Task<T?> GetUserDataAsync<T>(string userId, string userType, string key)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException(nameof(userId));
            }

            if (string.IsNullOrEmpty(userType))
            {
                throw new ArgumentException(nameof(userType));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException(nameof(key));
            }

            var data = await GetUserDataAsync(userId, userType) ?? new UserData(userId, userType);
            if (data.Data == null)
            {
                return default;
            }

            if (!data.Data.ContainsKey(key))
            {
                return default;
            }

            var dataObject = data.Data[key];
            if (dataObject is T obj)
            {
                return obj;
            }

            if (dataObject == default)
            {
                return default;
            }

            var serialized = m_Serializer.Serialize(dataObject);

            return m_Deserializer.Deserialize<T>(serialized);
        }

        public async Task SetUserDataAsync<T>(string userId, string userType, string key, T? value)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException(nameof(userId));
            }

            if (string.IsNullOrEmpty(userType))
            {
                throw new ArgumentException(nameof(userType));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException(nameof(key));
            }

            var userData = await GetUserDataAsync(userId, userType) ?? new UserData(userId, userType);
            userData.Data ??= new();

            if (userData.Data.ContainsKey(key))
            {
                userData.Data.Remove(key);
            }

            userData.Data.Add(key, value);
            await SetUserDataAsync(userData);
        }

        public async Task<IReadOnlyCollection<UserData>> GetUsersDataAsync(string type)
        {
            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentException(nameof(type));
            }

            var usersData = await GetUsersDataAsync();
            return usersData?
                       .Where(d => d.Type?.Equals(type, StringComparison.OrdinalIgnoreCase) ?? false)
                       .ToList()
                   ?? new List<UserData>();
        }

        public async Task SetUserDataAsync(UserData userData)
        {
            if (userData == null)
            {
                throw new ArgumentNullException(nameof(userData));
            }

            if (string.IsNullOrWhiteSpace(userData.Id))
            {
                throw new ArgumentException(
                    $"User data missing required property: {nameof(UserData.Id)}", nameof(userData));
            }

            if (string.IsNullOrWhiteSpace(userData.Type))
            {
                throw new ArgumentException(
                    $"User data missing required property: {nameof(UserData.Type)}", nameof(userData));
            }

            var usersData = await GetUsersDataAsync() ?? GetDefaultUsersData();

            var idx = usersData.FindIndex(c =>
                (c.Type?.Equals(userData.Type, StringComparison.OrdinalIgnoreCase) ?? false) &&
                (c.Id?.Equals(userData.Id, StringComparison.OrdinalIgnoreCase) ?? false));

            usersData.RemoveAll(c =>
                (c.Type?.Equals(userData.Type, StringComparison.OrdinalIgnoreCase) ?? false) &&
                (c.Id?.Equals(userData.Id, StringComparison.OrdinalIgnoreCase) ?? false));

            // preserve location in data
            if (idx >= 0)
            {
                usersData.Insert(idx, userData);
            }
            else
            {
                usersData.Add(userData);
            }

            m_CachedUsersData.Users = usersData;
            m_IsUpdating = true;

            await SaveAsync();
        }

        private List<UserData> GetDefaultUsersData()
        {
            return new()
            {
                new UserData
                {
                    FirstSeen = null,
                    LastSeen = null,
                    BanInfo = null,
                    LastDisplayName = "root",
                    Id = "root",
                    Type = KnownActorTypes.Rcon,
                    Data = new Dictionary<string, object?>(),
                    Permissions = new HashSet<string> { "*" },
                    Roles = new HashSet<string>()
                }
            };
        }

        private Task<List<UserData>?> GetUsersDataAsync()
        {
            return Task.FromResult(m_CachedUsersData.Users?.ToList());
        }

        private async Task<UsersData> LoadUsersDataFromDiskAsync()
        {
            if (!await m_DataStore.ExistsAsync(UsersKey))
            {
                m_CachedUsersData = new UsersData
                {
                    Users = GetDefaultUsersData()
                };

                await SaveAsync();
                return m_CachedUsersData;
            }

            return await m_DataStore.LoadAsync<UsersData>(UsersKey) ?? new UsersData
            {
                Users = GetDefaultUsersData()
            };
        }

        private async Task SaveAsync()
        {
            if (m_DataStore is YamlDataStore yamlDataStore)
            {
                await yamlDataStore.SaveAsync(
                    UsersKey,
                    m_CachedUsersData,
                    header: $"# yaml-language-server: $schema=./{SchemaConstants.UsersSchemaPath}\n"
                );

                return;
            }

            await m_DataStore.SaveAsync(UsersKey, m_CachedUsersData);
        }

        public async ValueTask DisposeAsync()
        {
            m_FileChangeWatcher.Dispose();

            if (m_CachedUsersData.Users == null)
            {
                throw new Exception("Tried to save null users data");
            }

            await SaveAsync();
        }
    }
}