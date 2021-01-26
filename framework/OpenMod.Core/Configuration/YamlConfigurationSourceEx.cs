using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using OpenMod.API;

namespace OpenMod.Core.Configuration
{
    /// <summary>
    /// A YAML file based <see cref="FileConfigurationSource"/>.
    /// Ex: Supports variables.
    /// </summary>
    [OpenModInternal]
    public class YamlConfigurationSourceEx : FileConfigurationSource
    {
        public IDictionary<string, string>? Variables { get; set; }

        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            return new YamlConfigurationProviderEx(this);
        }
    }
}