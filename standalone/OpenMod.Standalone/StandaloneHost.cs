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

            //todo: listen for host ready event and dont use a separate thread
            AsyncHelper.Schedule("InputLoop", InputLoop);

            return Task.CompletedTask;
        }

        private async Task InputLoop()
        {
            //Console.WriteLine("Ready. Type \"help\" for help.");
            //Console.Write("> ");

            string line;
            while (!(line = Console.ReadLine())?.Equals("exit", StringComparison.OrdinalIgnoreCase) ?? false)
            {
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                try
                {
                    await m_CommandExecutor.ExecuteAsync(m_ConsoleActorAccessor.Actor, line.Split(' ').ToArray(), string.Empty);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("> ");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString(), Color.DarkRed);
                    Debugger.Break();
                }
            }

            IsComponentAlive = false;
            await m_Runtime.ShutdownAsync();
        }

        public string DisplayName { get; } = "OpenMod Standalone Host";
        public string Version { get; } = "0.1.0";
        public string OpenModComponentId { get; } = "OpenMod.Standalone";
        public string WorkingDirectory { get; }
        public bool IsComponentAlive { get; private set; }
        public ILifetimeScope LifetimeScope { get; }
        public IDataStore DataStore { get; }
    }
}