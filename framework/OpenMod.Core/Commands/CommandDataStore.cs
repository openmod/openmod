using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.API.Ioc;
using OpenMod.API.Persistence;
using OpenMod.API.Prioritization;
using OpenMod.Common.Helpers;
using OpenMod.Core.Helpers;

namespace OpenMod.Core.Commands
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class CommandDataStore : ICommandDataStore, IAsyncDisposable
    {
        public const string CommandsKey = "commands";
        private readonly IDataStore m_DataStore;
        private readonly IRuntime m_Runtime;
        private readonly Lazy<ICommandStore> m_CommandStore;
        private readonly ILogger<CommandDataStore> m_Logger;
        private IDisposable? m_ChangeWatcher;
        private RegisteredCommandsData? m_Cache;

        public CommandDataStore(IOpenModDataStoreAccessor dataStoreAccessor,
            IRuntime runtime,
            Lazy<ICommandStore> commandStore,
            ILogger<CommandDataStore> logger)
        {
            m_DataStore = dataStoreAccessor.DataStore;
            m_Runtime = runtime;
            m_CommandStore = commandStore;
            m_Logger = logger;
        }

        public async Task<RegisteredCommandData?> GetRegisteredCommandAsync(string commandId)
        {
            if (string.IsNullOrEmpty(commandId))
            {
                throw new ArgumentException(nameof(commandId));
            }

            var commandsData = await GetRegisteredCommandsAsync();

            return commandsData.Commands?.FirstOrDefault(d =>
                d.Id?.Equals(commandId, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        public Task SetRegisteredCommandsAsync(RegisteredCommandsData data)
        {
            m_Cache = data ?? throw new ArgumentNullException(nameof(data));
            return m_DataStore.SaveAsync(CommandsKey, data);
        }

        public async Task SetCommandDataAsync<T>(string commandId, string key, T? value)
        {
            if (string.IsNullOrEmpty(commandId))
            {
                throw new ArgumentNullException(nameof(commandId));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            var commandData = await GetRegisteredCommandAsync(commandId) ?? new RegisteredCommandData();
            if (commandData.Data == null)
            {
                commandData.Data = new Dictionary<string, object?>();
            }
            else if (commandData.Data.ContainsKey(key))
            {
                commandData.Data.Remove(key);
            }

            commandData.Data.Add(key, value);
            await SetCommandDataAsync(commandData);
        }

        public async Task SetCommandDataAsync(RegisteredCommandData commandData)
        {
            if (commandData == null)
            {
                throw new ArgumentNullException(nameof(commandData));
            }

            var commandsData = await GetRegisteredCommandsAsync();
            var commands = commandsData.Commands ?? new List<RegisteredCommandData>();

            var idx = commands.FindIndex(c =>
                c.Id?.Equals(commandData.Id, StringComparison.OrdinalIgnoreCase) ?? false);

            commands.RemoveAll(c =>
                c.Id?.Equals(commandData.Id, StringComparison.OrdinalIgnoreCase) ?? false);

            // preserve location in data
            if (idx >= 0)
            {
                commands.Insert(idx, commandData);
            }
            else
            {
                commands.Add(commandData);
            }

            commandsData.Commands = commands;
            m_Cache = commandsData;
            await m_DataStore.SaveAsync(CommandsKey, commandsData);
        }

        public async Task<RegisteredCommandsData> GetRegisteredCommandsAsync()
        {
            if (m_Cache != null)
            {
                return m_Cache;
            }

            m_Cache = await LoadCommandsFromDisk();
            m_ChangeWatcher = m_DataStore.AddChangeWatcher(CommandsKey, m_Runtime, () =>
            {
                m_Logger.LogInformation("Commands have been reloaded");
                m_Cache = AsyncHelper.RunSync(async () =>
                {
                    var result = await LoadCommandsFromDisk();
                    await m_CommandStore.Value.InvalidateAsync();
                    return result;
                });
            });
            return m_Cache;
        }

        private async Task<RegisteredCommandsData> LoadCommandsFromDisk()
        {
            RegisteredCommandsData commandsData;
            if (await m_DataStore.ExistsAsync(CommandsKey))
            {
                commandsData = await m_DataStore.LoadAsync<RegisteredCommandsData>(CommandsKey)
                               ?? throw new InvalidOperationException("Failed to load commands data");

                if ((commandsData.Commands?.Count ?? 0) != 0)
                {
                    return commandsData;
                }
            }
            else
            {
                commandsData = GetDefaultCommandsData();
                await m_DataStore.SaveAsync(CommandsKey, m_Cache);
            }

            return commandsData;
        }

        private RegisteredCommandsData GetDefaultCommandsData()
        {
            return new()
            {
                Commands = new List<RegisteredCommandData>()
            };
        }

        public async Task<T?> GetCommandDataAsync<T>(string commandId, string key)
        {
            if (string.IsNullOrEmpty(commandId))
            {
                throw new ArgumentNullException(nameof(commandId));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            var data = await GetRegisteredCommandAsync(commandId);
            if (data?.Data == null || !data.Data.ContainsKey(key))
            {
                return default;
            }

            var dataObject = data.Data[key];
            if (dataObject is T obj)
            {
                return obj;
            }

            if (dataObject == null)
            {
                return default;
            }

            if (dataObject.GetType().HasConversionOperator(typeof(T)))
            {
                return (T)dataObject;
            }

            if (dataObject is Dictionary<object, object> dict)
            {
                return dict.ToObject<T>();
            }

            throw new Exception($"Failed to parse {dataObject.GetType()} as {typeof(T)}");
        }

        public ValueTask DisposeAsync()
        {
            m_ChangeWatcher?.Dispose();

            if (m_Cache?.Commands == null || m_Cache.Commands.Count == 0)
            {
                throw new Exception("Tried to save null or empty commands; this is a bug.");
            }

            return new ValueTask(m_DataStore.SaveAsync(CommandsKey, m_Cache));
        }
    }
}