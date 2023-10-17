using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using OpenMod.API;

namespace OpenMod.Core.Localization
{
    [OpenModInternal]
    public class ConfigurationBasedStringLocalizer : IStringLocalizer, IDisposable
    {
        private readonly IConfiguration m_Configuration;
        private readonly IOptions<SmartFormatOptions> m_Options;

        public ConfigurationBasedStringLocalizer(IConfiguration configuration, IOptions<SmartFormatOptions> options)
        {
            m_Configuration = configuration;
            m_Options = options;
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var list = new List<LocalizedString>();
            GetAllStrings(m_Configuration, list);
            return list;
        }

        private void GetAllStrings(IConfiguration configuration, List<LocalizedString> list)
        {
            foreach (var child in configuration.GetChildren())
            {
                if (child.GetChildren().Any())
                {
                    GetAllStrings(child, list);
                    continue;
                }

                list.Add(new LocalizedString(child.Path, child.Value ?? string.Empty));
            }
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return this; // no culture support
        }

        public LocalizedString this[string name]
        {
            get
            {
                var configValue = m_Configuration.GetSection(name);
                return !configValue.Exists() || string.IsNullOrEmpty(configValue.Value)
                    ? new LocalizedString(name, name, resourceNotFound: true)
                    : new LocalizedString(name, configValue.Value ?? string.Empty);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var configValue = m_Configuration.GetSection(name);
                if (!configValue.Exists() || string.IsNullOrEmpty(configValue.Value))
                {
                    return new LocalizedString(name, name, resourceNotFound: true);
                }

                var formatter = m_Options.Value.GetSmartFormatter();
                return new LocalizedString(name, formatter.Format(configValue.Value ?? string.Empty, arguments));
            }
        }

        public void Dispose()
        {
            (m_Configuration as IDisposable)?.Dispose();
        }
    }
}