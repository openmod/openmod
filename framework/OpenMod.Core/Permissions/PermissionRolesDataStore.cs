using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.API.Persistence;
using OpenMod.API.Prioritization;
using OpenMod.Core.Helpers;
using OpenMod.Core.Permissions.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMod.Common.Helpers;

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

        public List<PermissionRoleData> Roles { get => m_CachedPermissionRolesData!.Roles ?? new List<PermissionRoleData>(); }

        public PermissionRolesDataStore(
            ILogger<PermissionRolesDataStore> logger,
            IOpenModDataStoreAccessor dataStoreAccessor,
            IRuntime runtime)
        {
            m_DataStore = dataStoreAccessor.DataStore;
            m_Logger = logger;
            m_Runtime = runtime;
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

            if(dataObject == null)
            {
                return Task.FromResult<T?>(default);
            }

            if (dataObject.GetType().HasConversionOperator(typeof(T)))
            {
                // ReSharper disable once PossibleInvalidCastException
                return Task.FromResult<T?>((T)dataObject);
            }

            if (dataObject is Dictionary<object, object> dict)
            {
                return Task.FromResult(dict.ToObject<T?>());
            }

            throw new Exception($"Failed to parse {dataObject.GetType()} as {typeof(T)}");
        }

        public virtual async Task ReloadAsync()
        {
            m_Logger.LogInformation("Permissions have been reloaded");
            m_CachedPermissionRolesData = await m_DataStore.LoadAsync<PermissionRolesData>(RolesKey) ?? new PermissionRolesData();
        }

        public virtual Task SaveChangesAsync()
        {
            return m_DataStore.SaveAsync(RolesKey, m_CachedPermissionRolesData);
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