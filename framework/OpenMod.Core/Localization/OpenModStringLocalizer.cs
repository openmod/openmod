using System.Collections.Generic;
using System.Globalization;
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
    public class OpenModStringLocalizer : IOpenModStringLocalizer
    {
        private readonly IStringLocalizer m_StringLocalizer;

        public OpenModStringLocalizer(IStringLocalizerFactory stringLocalizerFactory, IRuntime runtime)
        {
            m_StringLocalizer = stringLocalizerFactory.Create("openmod.translations", runtime.WorkingDirectory);
        }
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return m_StringLocalizer.GetAllStrings(includeParentCultures);
        }

#pragma warning disable 618 // disable obsolete warning
        // lgtm [cs/call-to-obsolete-method]
        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return m_StringLocalizer.WithCulture(culture);
        }
#pragma warning restore 618

        public LocalizedString this[string name]
        {
            get
            {
                return m_StringLocalizer[name];
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                return m_StringLocalizer[name, arguments];
            }
        }
    }
}