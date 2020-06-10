using System;
using System.Collections.Generic;
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
    public class PermissionFileManager : IPermissionFileManager
    {
        public const string GroupsKey = "groups";
        public const string PlayersKey = "players";

        private readonly IDataStore m_DataStore;
        public PermissionFileManager(IOpenModDataStoreAccessor dataStoreAccessor)
        {
            m_DataStore = dataStoreAccessor.DataStore;
            AsyncHelper.RunSync(InitAsync);
        }

        private async Task InitAsync()
        {
            if (!await PermissionGroupsDataExistsAsync())
            {
                PermissionGroupsData = new PermissionGroupsData
                {
                    PermissionGroups = new List<PermissionGroupData>
                        {
                            new PermissionGroupData
                            {
                                Id = "default",
                                DisplayName = "Default",
                                Priority = 0,
                                Parents = new List<string>(),
                                Permissions = new List<string>
                                {
                                    "help"
                                },
                                Data = new Dictionary<string, object>(),
                                IsAutoAssigned = true
                            },
                            new PermissionGroupData
                            {
                                Id = "vip",
                                Priority = 1,
                                Parents = new List<string>
                                {
                                    "default"
                                },
                                Permissions = new List<string>
                                {
                                    "kit"
                                },
                                Data = new Dictionary<string, object>(),
                                IsAutoAssigned = false
                            }
                        }
                };
                await SavePermissionGroupsAsync();
            }
            else
            {
                await ReadPermissionGroupsAsync();
            }

            if (!await UsersDataExistsAsync())
            {
                // no need for default users data
                UsersData = new UsersData();
                await SaveUsersAsync();
            }
            else
            {
                await ReadUsersDataAsync();
            }
        }

        public PermissionGroupsData PermissionGroupsData { get; private set; }
        public UsersData UsersData { get; private set; }

        public async Task ReadPermissionGroupsAsync()
        {
            PermissionGroupsData = await m_DataStore.LoadAsync<PermissionGroupsData>(GroupsKey);
        }

        public async Task ReadUsersDataAsync()
        {
            UsersData = await m_DataStore.LoadAsync<UsersData>(PlayersKey);
        }

        public Task SavePermissionGroupsAsync()
        {
            return m_DataStore.SaveAsync(GroupsKey, PermissionGroupsData);
        }

        public Task SaveUsersAsync()
        {
            return m_DataStore.SaveAsync(PlayersKey, UsersData);
        }

        public Task<bool> PermissionGroupsDataExistsAsync()
        {
            return m_DataStore.ExistsAsync(GroupsKey);
        }

        public Task<bool> UsersDataExistsAsync()
        {
            return m_DataStore.ExistsAsync(PlayersKey);
        }
    }
}