using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.API.Ioc;
using OpenMod.API.Persistence;
using OpenMod.API.Prioritization;
using OpenMod.Core.Helpers;
using Semver;

namespace OpenMod.Standalone
{
    [UsedImplicitly]
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class StandaloneHost : IOpenModHost
    {
        private readonly IConsoleActorAccessor m_ConsoleActorAccessor;
        private readonly IRuntime m_Runtime;
        private readonly ICommandExecutor m_CommandExecutor;

        public StandaloneHost(
            IConsoleActorAccessor consoleActorAccessor,
            IRuntime runtime, 
            ICommandExecutor commandExecutor,
            ILifetimeScope lifetimeScope,
            IDataStoreFactory dataStoreFactory)
        {
            Version = VersionHelper.ParseAssemblyVersion(GetType().Assembly);
            m_ConsoleActorAccessor = consoleActorAccessor;
            m_Runtime = runtime;
            m_CommandExecutor = commandExecutor;
            WorkingDirectory = runtime.WorkingDirectory;
            LifetimeScope = lifetimeScope;
            DataStore = dataStoreFactory.CreateDataStore("openmod.console", WorkingDirectory);
        }

        public Task InitAsync()
        {
            IsComponentAlive = true;
            return Task.CompletedTask;
        }
        
        public string HostDisplayName { get; } = "OpenMod Standalone Host";
        public string HostVersion { get; } = "0.1.0";
        public SemVersion Version { get; }
        public string OpenModComponentId { get; } = "OpenMod.Standalone";
        public string WorkingDirectory { get; }
        public bool IsComponentAlive { get; private set; }
        public ILifetimeScope LifetimeScope { get; }
        public IDataStore DataStore { get; }
    }
}