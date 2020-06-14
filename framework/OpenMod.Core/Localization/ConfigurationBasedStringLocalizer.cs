using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using SmartFormat;

namespace OpenMod.Core.Localization
{
    public class ConfigurationBasedStringLocalizer : IStringLocalizer
    {
        private readonly IConfiguration m_Configuration;

        public ConfigurationBasedStringLocalizer(IConfiguration configuration)
        {
            m_Configuration = configuration;
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

                list.Add(new LocalizedString(child.Path, child.Value));
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
                return new LocalizedString(name, m_Configuration[name] ?? name);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var configValue = m_Configuration[name];
                return new LocalizedString(name, configValue != null ? Smart.Format(configValue, arguments) : name);
            }
        }
    }
}