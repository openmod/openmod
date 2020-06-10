using System;
using System.Drawing;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;

namespace OpenMod.Standalone
{
    [UsedImplicitly]
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class StandaloneHost : IOpenModHost
    {
        private readonly IRuntime m_Runtime;

        // todo: add commandhandler
        public StandaloneHost(IRuntime runtime)
        {
            m_Runtime = runtime;
            WorkingDirectory = runtime.WorkingDirectory;
        }

        public Task InitAsync()
        {
            IsComponentAlive = true;

            Task.Factory.StartNew(InputLoop);

            return Task.CompletedTask;
        }

        private async Task InputLoop()
        {
            string line;
            while (!(line = Console.ReadLine())?.Equals("exit", StringComparison.OrdinalIgnoreCase) ?? false)
            {
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                try
                {
                    // todo: handle commands
                    //if (!await cmdHandler.HandleCommandAsync(Console, line, ""))
                    //    Console.WriteLine("Command not found: " + line);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("> ");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                catch (Exception ex)

                {
                    Console.WriteLine(ex.ToString(), Color.DarkRed);
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
    }
}