using System;
using System.Threading.Tasks;
using Autofac;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.API.Ioc;
using OpenMod.API.Persistence;
using OpenMod.API.Prioritization;
using OpenMod.Core.Console;
using OpenMod.Core.Helpers;
using Semver;

namespace OpenMod.Standalone
{
    [UsedImplicitly]
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class StandaloneHost : IOpenModHost, IDisposable
    {
        public string HostDisplayName { get; } = "Standalone";

        public string Name { get; } = "OpenMod Standalone Host";

        public string HostVersion { get; } = "0.1.0";

        public SemVersion Version { get; }

        public string OpenModComponentId { get; } = "OpenMod.Standalone";

        public string WorkingDirectory { get; }

        public bool IsComponentAlive { get; private set; }

        public ILifetimeScope LifetimeScope { get; }

        public IDataStore DataStore { get; }

        private readonly IRuntime m_Runtime;
        private readonly IConsoleActorAccessor m_ConsoleActorAccessor;

        private readonly ICommandExecutor m_CommandExecutor;

        public StandaloneHost(
            IRuntime runtime, 
            ILifetimeScope lifetimeScope,
            IDataStoreFactory dataStoreFactory,
            IConsoleActorAccessor consoleActorAccessor,
            ICommandExecutor commandExecutor)
        {
            m_Runtime = runtime;
            m_ConsoleActorAccessor = consoleActorAccessor;
            m_CommandExecutor = commandExecutor;
            Version = VersionHelper.ParseAssemblyVersion(GetType().Assembly);
            WorkingDirectory = runtime.WorkingDirectory;
            LifetimeScope = lifetimeScope;

            DataStore = dataStoreFactory.CreateDataStore(new DataStoreCreationParameters
            {
                Component = this,
                Prefix = "openmod.standalone",
                Suffix = null,
                WorkingDirectory = WorkingDirectory
            });
        }

        public Task InitAsync()
        {
            IsComponentAlive = true;
            StandaloneConsoleIo.OnCommandExecute += OnCommandExecute;

            return Task.CompletedTask;
        }

        public Task ShutdownAsync()
        {
            Environment.Exit(0);
            return Task.CompletedTask;
        }

        public bool HasCapability(string capability)
        {
            return false;
        }

        public Task PerformHardReloadAsync()
        {
            // todo
            return m_Runtime.PerformSoftReloadAsync();
        }

        private void OnCommandExecute(string commandline)
        {
            var args = ArgumentsParser.ParseArguments(commandline);
            if (args.Length == 0)
            {
                return;
            }

            AsyncHelper.RunSync(() => m_CommandExecutor.ExecuteAsync(m_ConsoleActorAccessor.Actor, args, string.Empty));
        }

        public void Dispose()
        {
            StandaloneConsoleIo.OnCommandExecute -= OnCommandExecute;
        }
    }
}