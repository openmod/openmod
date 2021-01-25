using Microsoft.Extensions.Localization;
using OpenMod.API.Ioc;

namespace OpenMod.API.Localization
{
    /// <summary>
    /// The service used for localizing OpenMod messages.
    /// </summary>
    /// <remarks>
    /// This service is not used for localizing plugin messages.
    /// </remarks>
    [Service]
    public interface IOpenModStringLocalizer : IStringLocalizer
    {
        
    }
}