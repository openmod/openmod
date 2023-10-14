using Microsoft.Extensions.Configuration;
using OpenMod.Core.Files;

namespace OpenMod.Unturned.Configuration
{
    public class OpenModUnturnedConfiguration : IOpenModUnturnedConfiguration
    {
        public OpenModUnturnedConfiguration(string workingDirectory)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(workingDirectory)
                .AddYamlFile("openmod.unturned.yaml", optional: false, reloadOnChange: FileSettings.ReloadFilesOnChange)
                .Build();
        }

        public IConfiguration Configuration { get; }
    }
}