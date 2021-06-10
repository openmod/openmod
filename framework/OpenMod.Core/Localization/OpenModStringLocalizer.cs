using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.API.Localization;
using OpenMod.API.Prioritization;

namespace OpenMod.Core.Localization
{
    [OpenModInternal]
    [UsedImplicitly]
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class OpenModStringLocalizer : ProxyStringLocalizer, IOpenModStringLocalizer
    {
        public OpenModStringLocalizer(IStringLocalizerFactory stringLocalizerFactory, IRuntime runtime) : base(
            stringLocalizerFactory.Create("openmod.translations", runtime.WorkingDirectory))
        {
        }
    }
}