using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.API.Ioc;
using OpenMod.API.Permissions;
using OpenMod.API.Persistence;
using OpenMod.API.Prioritization;
using OpenMod.Common.Helpers;
using OpenMod.Core.Permissions;
using OpenMod.Core.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[assembly: RegisterPermission("cooldowns.immune", Description = "Grants immunity to all command cooldowns.")]

namespace OpenMod.Core.Cooldowns
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class CommandCooldownStore : ICommandCooldownStore
    {
        private const string c_DataStoreKey = "cooldowns";

        private readonly IConfiguration m_Configuration;
        private readonly IDataStore? m_DataStore;
        private readonly ILogger<CommandCooldownStore> m_Logger;
        private readonly IPermissionRoleStore m_PermissionRoleStore;
        private readonly IPermissionRolesDataStore m_PermissionRolesDataStore;
        private readonly Dictionary<string, List<CooldownRecord>> m_Records;
        private readonly IPermissionChecker m_PermissionChecker;
        private bool m_LoadedPersistedRecords;

        public CommandCooldownStore(
            IConfiguration configuration,
            IRuntime runtime,
            IDataStoreFactory dataStoreFactory,
            ILogger<CommandCooldownStore> logger,
            IPermissionRoleStore permissionRoleStore,
            IPermissionRolesDataStore permissionRolesDataStore,
            IPermissionChecker permissionChecker)
        {
            m_Configuration = configuration;
            m_DataStore = dataStoreFactory.CreateDataStore(new DataStoreCreationParameters()
            { Prefix = "openmod", WorkingDirectory = runtime.WorkingDirectory, LogOnChange = false });
            m_Logger = logger;
            m_PermissionRoleStore = permissionRoleStore;
            m_PermissionRolesDataStore = permissionRolesDataStore;
            m_PermissionChecker = permissionChecker;
            m_Records = new Dictionary<string, List<CooldownRecord>>();
            m_LoadedPersistedRecords = false;
        }

        public async Task<TimeSpan?> GetCooldownSpanAsync(ICommandActor actor, string commandId)
        {
            var roles = await m_PermissionRoleStore.GetRolesAsync(actor);

            if (roles.Count == 0)
            {
                return null;
            }

            TimeSpan? span = null;
            var priority = 0;

            foreach (var role in roles)
            {
                try
                {
                    // Skip as result won't matter
                    if (span.HasValue && priority >= role.Priority)
                    {
                        continue;
                    }

                    var data =
                        (await m_PermissionRolesDataStore.GetRoleDataAsync<List<object>>(role.Id,
                            "cooldowns"))?.OfType<Dictionary<object, object>>();

                    if (data == null)
                    {
                        continue;
                    }

                    foreach (var dict in data)
                    {
                        var currentSpan = dict.ToObject<CooldownSpan>();
                        if (currentSpan.Command?.Equals(commandId, StringComparison.OrdinalIgnoreCase) ?? false)
                        {
                            span = currentSpan.ToTimeSpan();
                            priority = role.Priority;
                        }
                    }
                }
                catch (Exception ex)
                {
                    m_Logger.LogError(ex, "Error occurred while parsing command cooldown");
                    throw;
                }
            }

            return span;
        }

        private string GetActorFullId(ICommandActor actor)
        {
            return actor.Type + "." + actor.Id;
        }

        private async Task LoadPersistedRecords(bool force = false)
        {
            if (m_LoadedPersistedRecords && !force) return;
            m_LoadedPersistedRecords = true;

            if (m_DataStore == null) return;

            if (!m_Configuration.GetValue("cooldowns:reloadPersistence", true)) return;

            if (!await m_DataStore.ExistsAsync("cooldowns")) return;

            var persistedRecords = (await m_DataStore.LoadAsync<CooldownRecords>(c_DataStoreKey))?.Records;

            if (persistedRecords == null || persistedRecords.Count == 0) return;

            foreach (var pair in persistedRecords)
            {
                if (pair.Value == null || pair.Value.Count == 0) continue;

                if (!m_Records.TryGetValue(pair.Key, out var records))
                {
                    records = new List<CooldownRecord>();

                    m_Records.Add(pair.Key, records);
                }

                foreach (var record in pair.Value)
                {
                    records.Add(record);
                }
            }
        }

        private async Task SavePersistedRecords()
        {
            if (m_DataStore == null) return;

            if (!m_Configuration.GetValue("cooldowns:reloadPersistence", true)) return;

            var persistedRecords = new CooldownRecords(m_Records);

            await m_DataStore.SaveAsync(c_DataStoreKey, persistedRecords);
        }

        public async Task<DateTime?> GetLastExecutedAsync(ICommandActor actor, string command)
        {
            if (actor.Type == KnownActorTypes.Console)
            {
                return null;
            }

            if (await m_PermissionChecker.CheckPermissionAsync(actor, "OpenMod.Core:cooldowns.immune") == PermissionGrantResult.Grant)
            {
                return null;
            }

            await LoadPersistedRecords();

            var actorId = GetActorFullId(actor);
            if (m_Records.TryGetValue(actorId, out List<CooldownRecord> records))
            {
                var record = records.FirstOrDefault(x => x.Command == command);

                if (record != null)
                {
                    return record.Executed;
                }
            }

            return null;
        }

        public async Task RecordExecutionAsync(ICommandActor actor, string command, DateTime time)
        {
            if (actor.Type == KnownActorTypes.Console) return;

            var actorId = GetActorFullId(actor);
            if (m_Records.TryGetValue(actorId, out var records))
            {
                var record = records.FirstOrDefault(x => x.Command == command);

                if (record == null)
                {
                    records.Add(new CooldownRecord(command, time));
                }
                else
                {
                    record.Executed = time;
                }
            }
            else
            {
                m_Records.Add(actorId, new List<CooldownRecord>
                {
                    new(command, time)
                });
            }

            await SavePersistedRecords();
        }
    }
}
