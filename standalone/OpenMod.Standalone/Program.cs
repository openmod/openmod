using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using OpenMod.API;
using System.Reflection;

namespace OpenMod.Standalone
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var path = Path.GetFullPath("openmod");
            await new Runtime.Runtime().InitAsync(new List<Assembly> { typeof(Program).Assembly }, new HostBuilder(), new RuntimeInitParameters
            {
                CommandlineArgs = args,
                WorkingDirectory = path
            });
        }
    }
}
