using System.IO;
using System.Threading.Tasks;

#if NUGET_BOOTSTRAP
using OpenMod.Runtime;
using System.Linq;
#else
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using OpenMod.API;
using System.Reflection;
#endif

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
