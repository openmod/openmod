using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using OpenMod.API;
using System.Reflection;

namespace OpenMod.Standalone
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var path = Path.GetFullPath("openmod");
            var runtime = new Runtime.Runtime();
            await runtime.InitAsync(new List<Assembly> { typeof(Program).Assembly }, new RuntimeInitParameters
            {
                CommandlineArgs = args,
                WorkingDirectory = path
            });

            StandaloneConsoleIo.StartListening();
        }
    }
}
