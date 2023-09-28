using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Ioc;
using OpenMod.API.Persistence;
using OpenMod.API.Prioritization;
using OpenMod.Core.Helpers;
using OpenMod.Core.Permissions.Data;
using OpenMod.Core.Persistence;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.Core.Configuration;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace OpenMod.Core.Permissions
{
    [OpenModInternal]
    [UsedImplicitly]
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class PermissionRolesDataStore : IPermissionRolesDataStore, IDisposable
    {
        public const string RolesKey = "roles";

        private readonly IDataStore m_DataStore;
        private readonly ILogger<PermissionRolesDataStore> m_Logger;
        private readonly IRuntime m_Runtime;
        private IDisposable? m_FileChangeWatcher;
        private PermissionRolesData? m_CachedPermissionRolesData;

        private readonly ISerializer m_Serializer;
        private readonly IDeserializer m_Deserializer;

        public List<PermissionRoleData> Roles { get => m_CachedPermissionRolesData!.Roles ?? new List<PermissionRoleData>(); }

        public PermissionRolesDataStore(
            ILogger<PermissionRolesDataStore> logger,
            IOpenModDataStoreAccessor dataStoreAccessor,
            IRuntime runtime,
            IEventBus eventBus)
        {
            m_DataStore = dataStoreAccessor.DataStore;
            m_Logger = logger;
            m_Runtime = runtime;

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

            AsyncHelper.RunSync(InitAsync);
        }

        protected virtual async Task InitAsync()
        {
            if (!await ExistsAsync())
            {
                m_CachedPermissionRolesData = new PermissionRolesData
                {
                    Roles = new List<PermissionRoleData>
                    {
                        new()
                        {
                            Id = "default",
                            DisplayName = "Default",
                            Priority = 0,
                            Data = new Dictionary<string, object?>(),
                            Parents = new HashSet<string>(),
                            Permissions = new HashSet<string>
                            {
                                "OpenMod.Core:commands.help"
                            },
                            IsAutoAssigned = true
                        },
                        new ()
                        {
                            Id = "vip",
                            Priority = 1,
                            Parents = new HashSet<string>
                            {
                                "default"
                            },
                            Data = new Dictionary<string, object?>(),
                            DisplayName = "VIP",
                            Permissions = new HashSet<string>
                            {
                                "SomeKitsPlugin:kits.vip"
                            },
                            IsAutoAssigned = false
                        }
                    }
                };

                await SaveChangesAsync();
            }
            else
            {
                await ReloadAsync();
            }

            m_FileChangeWatcher = m_DataStore.AddChangeWatcher(RolesKey, m_Runtime, () => AsyncHelper.RunSync(ReloadAsync));
        }

        public Task<PermissionRoleData?> GetRoleAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException(nameof(id));
            }

            var role = Roles.Find(d => d.Id?.Equals(id, StringComparison.OrdinalIgnoreCase) ?? false);
            return Task.FromResult<PermissionRoleData?>(role);
        }

        public Task<T?> GetRoleDataAsync<T>(string roleId, string key)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                throw new ArgumentException(nameof(roleId));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException(nameof(key));
            }

            var role = Roles.Find(d => d.Id?.Equals(roleId, StringComparison.OrdinalIgnoreCase) ?? false);
            if (role == null)
            {
                return Task.FromException<T?>(new Exception($"Role does not exist: {roleId}"));
            }

            if (role.Data == null || !role.Data.ContainsKey(key))
            {
                return Task.FromResult<T?>(default);
            }

            var dataObject = role.Data[key];
            if (dataObject is T obj)
            {
                return Task.FromResult<T?>(obj);
            }

            if (dataObject == default)
            {
                return Task.FromResult<T?>(default);
            }

            var serialized = m_Serializer.Serialize(dataObject);

            return Task.FromResult<T?>(m_Deserializer.Deserialize<T>(serialized));
        }

        public virtual async Task ReloadAsync()
        {
            m_Logger.LogInformation("Permissions have been reloaded");
            m_CachedPermissionRolesData = await m_DataStore.LoadAsync<PermissionRolesData>(RolesKey) ?? new PermissionRolesData();
        }

        public virtual async Task SaveChangesAsync()
        {
            if (m_DataStore is YamlDataStore yamlDataStore)
            {
                await yamlDataStore.SaveAsync(
                    RolesKey,
                    m_CachedPermissionRolesData,
                    header: $"# yaml-language-server: $schema=./{SchemaConstants.RolesSchemaPath}\n"
                );

                return;
            }

            await m_DataStore.SaveAsync(RolesKey, m_CachedPermissionRolesData);
        }

        public virtual Task<bool> ExistsAsync()
        {
            return m_DataStore.ExistsAsync(RolesKey);
        }

        public void Dispose()
        {
            m_FileChangeWatcher?.Dispose();
        }
    }
}