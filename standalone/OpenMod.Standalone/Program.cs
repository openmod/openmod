using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.Runtime;

namespace OpenMod.Standalone
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var path = Path.GetFullPath("openmod");

#if NUGET_BOOTSTRAP
            await new OpenModDynamicBootstrapper().BootstrapAsync(path, args, "OpenMod.Core", args.Contains("-Pre"));
#else
            await new Runtime.Runtime().InitAsync(new List<Assembly> { typeof(Program).Assembly }, new HostBuilder(), new RuntimeInitParameters
            {
                CommandlineArgs = args,
                WorkingDirectory = path
            });
#endif
        }
    }

    [UsedImplicitly]
    [ServiceImplementation]
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
