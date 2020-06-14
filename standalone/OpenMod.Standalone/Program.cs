using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Microsoft.Extensions.Hosting;
using OpenMod.API;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Commands;

namespace OpenMod.Standalone
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var path = Path.GetFullPath("openmod");
            var runtime = new Runtime.Runtime();
            var host = await runtime.InitAsync(new List<Assembly> { typeof(Program).Assembly }, new HostBuilder(), new RuntimeInitParameters
            {
                CommandlineArgs = args,
                WorkingDirectory = path
            });

            var commandExecutor = host.Services.GetRequiredService<ICommandExecutor>();
            var consoleActorAccessor = host.Services.GetRequiredService<IConsoleActorAccessor>();

            string line;
            while (!(line = Console.ReadLine())?.Equals("exit", StringComparison.OrdinalIgnoreCase) ?? false)
            {
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                try
                {
                    await commandExecutor.ExecuteAsync(consoleActorAccessor.Actor, line.Split(' ').ToArray(), string.Empty);
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
        }
    }
}
