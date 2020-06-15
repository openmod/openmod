using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace OpenMod.API.Ioc
{
    public interface IConfigurationConfigurator
    {
        Task ConfigureConfigurationAsync(IOpenModStartupContext openModStartupContext, IConfigurationBuilder configurationBuilder);
    }
}