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

namespace OpenMod.Core.Permissions
{
    [UsedImplicitly]
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class PermissionGroupsDataStore : IPermissionGroupsDataStore
    {
        public const string GroupsKey = "groups";

        private readonly IDataStore m_DataStore;
        public PermissionGroupsDataStore(IOpenModDataStoreAccessor dataStoreAccessor)
        {
            m_DataStore = dataStoreAccessor.DataStore;
            AsyncHelper.RunSync(InitAsync);
        }

        protected virtual async Task InitAsync()
        {
            if (!await ExistsAsync())
            {
                PermissionGroups = new List<PermissionGroupData>
                {
                    new PermissionGroupData
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
                    new PermissionGroupData
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

        public List<PermissionGroupData> PermissionGroups { get; private set; }
        public Task<PermissionGroupData> GetGroupAsync(string id)
        {
            var group = PermissionGroups.FirstOrDefault(d => d.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(group);
        }

        public virtual async Task ReloadAsync()
        {
            PermissionGroups = (await m_DataStore.LoadAsync<PermissionGroupsData>(GroupsKey)).PermissionGroups ?? new List<PermissionGroupData>();
        }

        public virtual Task SaveChangesAsync()
        {
            return m_DataStore.SaveAsync(GroupsKey, new PermissionGroupsData { PermissionGroups = PermissionGroups });
        }

        public virtual Task<bool> ExistsAsync()
        {
            return m_DataStore.ExistsAsync(GroupsKey);
        }
    }
}