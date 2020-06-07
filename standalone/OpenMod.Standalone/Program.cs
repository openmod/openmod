using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Hosting;
using OpenMod.API;
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
}
