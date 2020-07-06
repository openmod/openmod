using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;

namespace OpenMod.Core.Localization
{
    public class ConfigurationBasedStringLocalizerFactory : IStringLocalizerFactory
    {
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

            return new ConfigurationBasedStringLocalizer(translations);
        }
    }
}