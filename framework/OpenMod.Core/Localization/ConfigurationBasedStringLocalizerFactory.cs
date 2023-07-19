using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenMod.API;

namespace OpenMod.Core.Localization
{
    [OpenModInternal]
    public class ConfigurationBasedStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly ILogger<ConfigurationBasedStringLocalizer> m_Logger;
        private readonly IOptions<SmartFormatOptions> m_Options;

        public ConfigurationBasedStringLocalizerFactory(ILogger<ConfigurationBasedStringLocalizer> logger, IOptions<SmartFormatOptions> options)
        {
            m_Logger = logger;
            m_Options = options;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            throw new NotSupportedException("Please use Create(baseName, location) instead!");
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            if (!Directory.Exists(location))
            {
                Directory.CreateDirectory(location);
            }

            var translations = new ConfigurationBuilder()
                .SetBasePath(location)
                .AddYamlFile(baseName + ".yaml", true, reloadOnChange: true)
                .Build();

            void ReloadToken()
            {
                translations.GetReloadToken().RegisterChangeCallback(_ =>
                {
                    m_Logger.LogInformation("Reloading translations: {Path}", Path.Combine(location, baseName + ".yaml"));
                    ReloadToken();
                }, null);
            }

            ReloadToken();

            return new ConfigurationBasedStringLocalizer(translations, m_Options);
        }
    }
}