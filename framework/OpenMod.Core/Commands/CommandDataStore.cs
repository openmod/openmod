using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.API.Persistence;
using OpenMod.API.Prioritization;
using OpenMod.Core.Helpers;

namespace OpenMod.Core.Commands
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class CommandDataStore : ICommandDataStore, IAsyncDisposable
    {
        public const string CommandsKey = "commands";
        private readonly IDataStore m_DataStore;
        private readonly IRuntime m_Runtime;
        private IDisposable m_ChangeWatcher;
        private RegisteredCommandsData m_Cache;

        public CommandDataStore(IOpenModDataStoreAccessor dataStoreAccessor, IRuntime runtime)
        {
            m_DataStore = dataStoreAccessor.DataStore;
            m_Runtime = runtime;
        }

        public async Task<RegisteredCommandData> GetRegisteredCommandAsync(string commandId)
        {
            var commandsData = await GetRegisteredCommandsAsync();
            return commandsData.Commands.FirstOrDefault(d =>
                d.Id.Equals(commandId, StringComparison.OrdinalIgnoreCase));
        }

        public Task SetRegisteredCommandsAsync(RegisteredCommandsData data)
        {
            m_Cache = data;
            return Task.CompletedTask;
        }

        public async Task SetCommandDataAsync<T>(string commandId, string key, T value)
        {
            var commandData = await GetRegisteredCommandAsync(commandId);
            if (commandData.Data == null)
            {
                commandData.Data = new Dictionary<string, object>();
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
            var commandsData = await GetRegisteredCommandsAsync();
            var idx = commandsData.Commands.FindIndex(c =>
                c.Id.Equals(commandData.Id, StringComparison.OrdinalIgnoreCase));

            commandsData.Commands.RemoveAll(c =>
                c.Id.Equals(commandData.Id, StringComparison.OrdinalIgnoreCase));

            // preserve location in data
            if (idx >= 0)
            {
                commandsData.Commands.Insert(idx, commandData);
            }
            else
            {
                commandsData.Commands.Add(commandData);
            }

            m_Cache = commandsData;
        }

        public async Task<RegisteredCommandsData> GetRegisteredCommandsAsync()
        {
            if(m_Cache != null)
            {
                return m_Cache;
            }
            m_Cache = await LoadCommandsFromDisk();
            m_ChangeWatcher = m_DataStore.AddChangeWatcher(CommandsKey, m_Runtime, () =>
            {
                m_Cache = AsyncHelper.RunSync(LoadCommandsFromDisk);
            });
            return m_Cache;
        }

        private async Task<RegisteredCommandsData> LoadCommandsFromDisk()
        {
            return await m_DataStore.LoadAsync<RegisteredCommandsData>(CommandsKey) ?? new RegisteredCommandsData
            {
                Commands = new List<RegisteredCommandData>()
            };
        }

        public async Task<T> GetCommandDataAsync<T>(string commandId, string key)
        {
            var data = await GetRegisteredCommandAsync(commandId);
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

        public ValueTask DisposeAsync()
        {
            m_ChangeWatcher?.Dispose();
            return new ValueTask(m_DataStore.SaveAsync(CommandsKey, m_Cache));
        }
    }
}