using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.API.Ioc;
using OpenMod.API.Permissions;
using OpenMod.API.Prioritization;
using OpenMod.Core.Helpers;
using OpenMod.Core.Prioritization;

namespace OpenMod.Core.Commands
{
    [OpenModInternal]
    [UsedImplicitly]
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class CommandStore : ICommandStore, IAsyncDisposable
    {
        private bool m_IsDisposing;

        private readonly PriorityComparer m_Comparer;
        private readonly IOptions<CommandStoreOptions> m_Options;
        private readonly IRuntime m_Runtime;
        private readonly IServiceProvider m_ServiceProvider;
        private readonly IPermissionRegistry m_PermissionRegistry;
        private readonly ICommandPermissionBuilder m_CommandPermissionBuilder;
        private readonly ICommandDataStore m_CommandDataStore;
        private readonly ILogger<CommandStore> m_Logger;
        private IReadOnlyCollection<ICommandSource> m_CommandSources;

        public CommandStore(IOptions<CommandStoreOptions> options,
            IRuntime runtime,
            IServiceProvider serviceProvider,
            IPermissionRegistry permissionRegistry,
            ICommandPermissionBuilder commandPermissionBuilder,
            ICommandDataStore commandDataStore,
            ILogger<CommandStore> logger)
        {
            m_Comparer = new PriorityComparer(PriortyComparisonMode.HighestFirst);
            m_CommandSources = options.Value.CreateCommandSources(serviceProvider);
            m_Options = options;
            m_Runtime = runtime;
            m_ServiceProvider = serviceProvider;
            m_PermissionRegistry = permissionRegistry;
            m_CommandPermissionBuilder = commandPermissionBuilder;
            m_CommandDataStore = commandDataStore;
            m_Logger = logger;

            options.Value.OnCommandSourcesChanged += OnCommandSourcesChanged;
            OnCommandSourcesChanged();
        }

        private void OnCommandSourcesChanged()
        {
            AsyncHelper.RunSync(InvalidateAsync);
        }

        public async Task InvalidateAsync()
        {
            await m_CommandSources.DisposeAllAsync();

            if (m_Runtime.IsDisposing)
            {
                return;
            }

            m_CommandSources = m_Options.Value.CreateCommandSources(m_ServiceProvider);

            if (m_CommandSources.Count == 0)
            {
                m_Logger.LogDebug("InvalidateAsync: failed because no command sources were found; this is normal on booting");
                return;
            }

            var commands = new List<ICommandRegistration>();
            foreach (var sources in m_CommandSources)
            {
                commands.AddRange(await sources.GetCommandsAsync());
            }

            foreach (var registration in commands)
            {
                var permission = m_CommandPermissionBuilder.GetPermission(registration, commands).Split(':')[1];

                m_PermissionRegistry.RegisterPermission(registration.Component,
                    permission,
                    description: $"Grants access to the {registration.Id} command.",
                    defaultGrant: PermissionGrantResult.Default);

                if (registration.PermissionRegistrations == null)
                {
                    continue;
                }

                foreach (var permissionRegistration in registration.PermissionRegistrations)
                {
                    m_PermissionRegistry.RegisterPermission(permissionRegistration.Owner,
                        $"{permission}.{permissionRegistration.Permission}",
                        permissionRegistration.Description,
                        permissionRegistration.DefaultGrant);
                }
            }

            var commandsData = await m_CommandDataStore.GetRegisteredCommandsAsync();
            if (commandsData?.Commands == null)
            {
                throw new Exception("Failed to register commands: command data was null");
            }

            foreach (var command in commands
                .Where(d => !commandsData.Commands.Any(c =>
                    c.Id?.Equals(d.Id, StringComparison.OrdinalIgnoreCase) ?? false)))
            {
                commandsData.Commands.Add(CreateDefaultCommandData(command));
            }

            if (commandsData.Commands.Count == 0)
            {
                throw new Exception("Failed to register commands: command data was empty.");
            }

            await m_CommandDataStore.SetRegisteredCommandsAsync(commandsData);
            m_Logger.LogDebug("Reloaded {Count} commands", commands.Count);
        }

        public async Task<IReadOnlyCollection<ICommandRegistration>> GetCommandsAsync()
        {
            if (m_IsDisposing)
            {
                throw new ObjectDisposedException(nameof(CommandStore));
            }

            var commands = new List<ICommandRegistration>();
            foreach (var sources in m_CommandSources)
            {
                foreach (var command in await sources.GetCommandsAsync())
                {
                    if (!command.IsEnabled)
                    {
                        continue;
                    }

                    if (!command.Component.IsComponentAlive)
                    {
                        continue;
                    }

                    var commandData = await GetOrCreateCommandData(command);
                    if (!commandData.Enabled ?? true)
                    {
                        continue;
                    }
                    commands.Add(new RegisteredCommand(command, commandData));
                }
            }

            commands.Sort((a, b) => m_Comparer.Compare(a.Priority, b.Priority));
            return commands;
        }

        private async Task<RegisteredCommandData> GetOrCreateCommandData(ICommandRegistration command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            var commandData = await m_CommandDataStore.GetRegisteredCommandAsync(command.Id);
            return commandData ?? CreateDefaultCommandData(command);
        }

        private RegisteredCommandData CreateDefaultCommandData(ICommandRegistration command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            return new RegisteredCommandData
            {
                Name = command.Name,
                Priority = command.Priority,
                Data = new Dictionary<string, object?>(),
                ParentId = command.ParentId,
                Aliases = command.Aliases?.ToList(),
                Enabled = true,
                Id = command.Id
            };
        }

        public async ValueTask DisposeAsync()
        {
            if (m_IsDisposing)
            {
                return;
            }

            m_Options.Value.OnCommandSourcesChanged -= OnCommandSourcesChanged;

            m_IsDisposing = true;
            var commandsData = await m_CommandDataStore.GetRegisteredCommandsAsync();
            if (commandsData?.Commands != null && commandsData.Commands.Count > 0)
            {
                // clear unknown commands

                var commands = new List<ICommandRegistration>();
                foreach (var sources in m_CommandSources)
                {
                    commands.AddRange(await sources.GetCommandsAsync());
                }

                var isDirty = false;
                foreach (var command in commandsData.Commands
                    .Where(command => !commands.Any(d => d.Id.Equals(command.Id, StringComparison.OrdinalIgnoreCase))).ToList())
                {
                    commandsData.Commands.Remove(command);
                    isDirty = true;
                }

                if (isDirty)
                {
                    await m_CommandDataStore.SetRegisteredCommandsAsync(commandsData);
                }
            }

            await m_CommandSources.DisposeAllAsync();
        }
    }
}