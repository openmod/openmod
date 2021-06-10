using Microsoft.Extensions.Localization;
using OpenMod.API.Ioc;

namespace OpenMod.API.Localization
{
    /// <summary>
    /// The service used for localizing the OpenMod host's messages.
    /// </summary>
    /// <remarks>
    /// This service is not used for localizing plugin messages.
    /// </remarks>
    [Service]
    public interface IOpenModHostStringLocalizer : IStringLocalizer
    {
    }
}
