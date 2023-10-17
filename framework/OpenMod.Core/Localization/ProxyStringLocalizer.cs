using Microsoft.Extensions.Localization;
using OpenMod.API;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace OpenMod.Core.Localization
{
    [OpenModInternal]
    public class ProxyStringLocalizer : IStringLocalizer, IDisposable
    {
        private readonly IStringLocalizer m_StringLocalizer;

        public ProxyStringLocalizer(IStringLocalizer stringLocalizer)
        {
            m_StringLocalizer = stringLocalizer;
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return m_StringLocalizer.GetAllStrings(includeParentCultures);
        }

#pragma warning disable 618 // disable obsolete warning
        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return m_StringLocalizer.WithCulture(culture);
        }
#pragma warning restore 618

        public LocalizedString this[string name]
        {
            get { return m_StringLocalizer[name]; }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get { return m_StringLocalizer[name, arguments]; }
        }

        public void Dispose()
        {
            (m_StringLocalizer as IDisposable)?.Dispose();
        }
    }
}
