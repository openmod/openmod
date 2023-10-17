using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace OpenMod.Unturned.Configuration
{
    public class OpenModUnturnedConfiguration : IOpenModUnturnedConfiguration
    {
        public OpenModUnturnedConfiguration(HostBuilderContext context)
        {
            Configuration = context.Configuration;
        }

        public IConfiguration Configuration { get; }
    }
}