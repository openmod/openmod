using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.API.Persistence;
using OpenMod.API.Prioritization;
using OpenMod.Core.Helpers;
using OpenMod.Core.Permissions.Data;

namespace OpenMod.Core.Permissions
{
    [OpenModInternal]
    [UsedImplicitly]
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class PermissionRolesDataStore : IPermissionRolesDataStore
    {
        public const string RolesKey = "roles";

        private readonly IDataStore m_DataStore;
        public PermissionRolesDataStore(IOpenModDataStoreAccessor dataStoreAccessor)
        {
            m_DataStore = dataStoreAccessor.DataStore;
            AsyncHelper.RunSync(InitAsync);
        }

        protected virtual async Task InitAsync()
        {
            if (!await ExistsAsync())
            {
                Roles = new List<PermissionRoleData>
                {
                    new PermissionRoleData
                    {
                        Id = "default",
                        DisplayName = "Default",
                        Priority = 0,
                        Permissions = new HashSet<string> 
                        {
                            "help"
                        },
                        IsAutoAssigned = true
                    },
                    new PermissionRoleData
                    {
                        Id = "vip",
                        Priority = 1,
                        Parents = new HashSet<string>
                        {
                            "default"
                        },
                        Permissions = new HashSet<string>
                        {
                            "kit.vip"
                        },
                        IsAutoAssigned = false
                    }
                };
                await SaveChangesAsync();
            }
            else
            {
                await ReloadAsync();
            }
        }

        public List<PermissionRoleData> Roles { get; private set; }
        public Task<PermissionRoleData> GetRoleAsync(string id)
        {
            var role = Roles.FirstOrDefault(d => d.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(role);
        }

        public Task<T> GetRoleDataAsync<T>(string roleId, string key)
        {
            var roleData = Roles.FirstOrDefault(d => d.Id.Equals(roleId, StringComparison.OrdinalIgnoreCase));
            if (roleData == null)
            {
                return Task.FromException<T>(new Exception($"Role does not exist: {roleId}"));
            }

            if (!roleData.Data.ContainsKey(key))
            {
                return Task.FromResult<T>(default);
            }

            var dataObject = roleData.Data[key];

            if (dataObject is T obj)
            {
                return Task.FromResult(obj);
            }

            if (dataObject.GetType().HasConversionOperator(typeof(T)))
            {
                // ReSharper disable once PossibleInvalidCastException
                return Task.FromResult((T)dataObject);
            }

            if (dataObject is Dictionary<object, object> dict)
            {
                return Task.FromResult(dict.ToObject<T>());
            }

            throw new Exception($"Failed to parse {dataObject.GetType()} as {typeof(T)}");
        }

        public virtual async Task ReloadAsync()
        {
            Roles = (await m_DataStore.LoadAsync<PermissionRolesData>(RolesKey))?.Roles ?? new List<PermissionRoleData>();
        }

        public virtual Task SaveChangesAsync()
        {
            return m_DataStore.SaveAsync(RolesKey, new PermissionRolesData { Roles = Roles });
        }

        public virtual Task<bool> ExistsAsync()
        {
            return m_DataStore.ExistsAsync(RolesKey);
        }
    }
}