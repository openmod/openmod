using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using OpenMod.API;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace OpenMod.Standalone
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var path = Path.Combine(AppContext.BaseDirectory, "openmod");
            var runtime = new Runtime.Runtime();
            var host = await runtime.InitAsync(new List<Assembly> { typeof(Program).Assembly }, new RuntimeInitParameters
            {
                CommandlineArgs = args,
                WorkingDirectory = path
            });

            var applifecycle = host.Services.GetRequiredService<IHostApplicationLifetime>();
            applifecycle.ApplicationStopping.Register(StandaloneConsoleIo.StopListening);

            var autoCompletionHandler = host.Services.GetRequiredService<IAutoCompleteHandler>();
            ReadLine.AutoCompletionHandler = autoCompletionHandler;
            ReadLine.HistoryEnabled = true;

            StandaloneConsoleIo.StartListening();
        }
    }
}
