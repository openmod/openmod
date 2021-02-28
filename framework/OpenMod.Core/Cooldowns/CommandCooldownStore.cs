using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API.Commands;
using OpenMod.API.Ioc;
using OpenMod.API.Permissions;
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
        private readonly ILogger<CommandCooldownStore> m_Logger;
        private readonly IPermissionRoleStore m_PermissionRoleStore;
        private readonly IPermissionRolesDataStore m_PermissionRolesDataStore;
        private readonly Dictionary<string, List<CooldownRecord>> m_Records;
        private readonly IPermissionChecker m_PermissionChecker;

        public CommandCooldownStore(
            ILogger<CommandCooldownStore> logger,
            IPermissionRoleStore permissionRoleStore,
            IPermissionRolesDataStore permissionRolesDataStore,
            IPermissionChecker permissionChecker)
        {
            m_Logger = logger;
            m_PermissionRoleStore = permissionRoleStore;
            m_PermissionRolesDataStore = permissionRolesDataStore;
            m_PermissionChecker = permissionChecker;
            m_Records = new Dictionary<string, List<CooldownRecord>>();
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

        private Task<List<CooldownRecord>?> GetPersistedRecords(string actorId, bool force = false)
        {
            // todo: implement openmod.cooldowns.yaml
            return Task.FromResult<List<CooldownRecord>?>(null);
        }

        private Task SavePersistedRecord(string actorId, string command, DateTime time)
        {
            // todo: implement openmod.cooldowns.yaml
            return Task.CompletedTask;
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

            var actorId = GetActorFullId(actor);
            if (m_Records.TryGetValue(actorId, out List<CooldownRecord> records))
            {
                var record = records.FirstOrDefault(x => x.Command == command);

                if (record != null)
                {
                    return record.Executed;
                }
            }

            return (await GetPersistedRecords(actorId))?.FirstOrDefault(x => x.Command == command)?.Executed;
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

            await SavePersistedRecord(actorId, command, time);
        }
    }
}
