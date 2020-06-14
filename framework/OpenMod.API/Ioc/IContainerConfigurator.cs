using System.Threading.Tasks;
using Autofac;

namespace OpenMod.API.Ioc
{
    public interface IContainerConfigurator
    {
        Task ConfigureContainerAsync(IOpenModStartupContext openModStartupContext, ContainerBuilder containerBuilder);
    }
}