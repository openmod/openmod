extern alias JetBrainsAnnotations;
using JetBrainsAnnotations::JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.API.Localization;
using OpenMod.API.Prioritization;
using OpenMod.Core.Localization;

namespace OpenMod.Unturned.Localization
{
    [OpenModInternal]
    [UsedImplicitly]
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class OpenModStringLocalizer : ProxyStringLocalizer, IOpenModHostStringLocalizer
    {
        public OpenModStringLocalizer(IStringLocalizerFactory stringLocalizerFactory, IRuntime runtime) : base(
            stringLocalizerFactory.Create("openmod.unturned.translations", runtime.WorkingDirectory))
        {
        }
    }
}