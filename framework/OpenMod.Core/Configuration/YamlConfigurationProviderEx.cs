using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using OpenMod.API;
using OpenMod.Common.Helpers;
using VYaml.Serialization;

namespace OpenMod.Core.Configuration
{
    /// <summary>
    /// A YAML file based <see cref="FileConfigurationProvider"/>.
    /// Ex: Supports variables.
    /// </summary>
    [OpenModInternal]
    public class YamlConfigurationProviderEx : FileConfigurationProvider
    {
        public YamlConfigurationProviderEx(YamlConfigurationSourceEx source) : base(source)
        { }

        public override void Load(Stream stream)
        {
            using var reader = new StreamReader(stream, detectEncodingFromByteOrderMarks: true);
            var yaml = reader.ReadToEnd();

            PreProcessYaml(ref yaml);

            using var outStream = new MemoryStream();
            using var writer = new StreamWriter(outStream, reader.CurrentEncoding);
            writer.Write(yaml);
            writer.Flush();
            outStream.Seek(0, SeekOrigin.Begin);

            try
            {
                Data = YamlSerializer.Deserialize<IDictionary<string, string?>>(outStream.ReadAllBytes());
            }
            catch (YamlSerializerException e)
            {
                throw new FormatException($"Could not parse the YAML file: {e.Message}.", e);
            }
        }

        private void PreProcessYaml(ref string yaml)
        {
            if (Source is not YamlConfigurationSourceEx sourceEx || sourceEx.Variables == null)
            {
                return;
            }

            foreach (var variable in sourceEx.Variables)
            {
                yaml = yaml.Replace("{{" + variable.Key + "}}", variable.Value);
            }
        }
    }
}
