using Microsoft.Extensions.Configuration;
using OpenMod.API.Ioc;

namespace OpenMod.Unturned.Configuration
{
    [Service]
    public interface IOpenModUnturnedConfiguration
    {
        IConfiguration Configuration { get; }
    }
}