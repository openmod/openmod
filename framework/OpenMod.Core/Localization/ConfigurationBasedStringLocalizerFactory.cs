using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OpenMod.API;

namespace OpenMod.Core.Localization
{
    [OpenModInternal]
    public class ConfigurationBasedStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly ILogger<ConfigurationBasedStringLocalizer> m_Logger;

        public ConfigurationBasedStringLocalizerFactory(ILogger<ConfigurationBasedStringLocalizer> logger)
        {
            m_Logger = logger;
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

            var reloadToken = translations.GetReloadToken();
            reloadToken.RegisterChangeCallback(_ =>
            {
                m_Logger.LogInformation("Reloading translations: {Path}", Path.Combine(location, baseName + ".yaml"));
            }, null);

            return new ConfigurationBasedStringLocalizer(translations);
        }
    }
}