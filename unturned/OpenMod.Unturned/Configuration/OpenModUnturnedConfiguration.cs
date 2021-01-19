using Microsoft.Extensions.Configuration;

namespace OpenMod.Unturned.Configuration
{
    public class OpenModUnturnedConfiguration : IOpenModUnturnedConfiguration
    {
        public OpenModUnturnedConfiguration(string workingDirectory)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(workingDirectory)
                .AddYamlFile("openmod.unturned.yaml", optional: false, reloadOnChange: true)
                .Build();
        }

        public IConfiguration Configuration { get; }
    }
}