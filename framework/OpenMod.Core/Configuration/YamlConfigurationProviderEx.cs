using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using NetEscapades.Configuration.Yaml;
using OpenMod.API;
using YamlDotNet.Core;

namespace OpenMod.Core.Configuration
{
    /// <summary>
    /// A YAML file based <see cref="FileConfigurationProvider"/>.
    /// Ex: Supports variables.
    /// </summary>
    [OpenModInternal]
    public class YamlConfigurationProviderEx : FileConfigurationProvider
    {
        private readonly YamlConfigurationSourceEx m_Source;
        private static readonly Type s_ParserType;
        private static readonly MethodInfo s_ParseMethod;

        static YamlConfigurationProviderEx()
        {
            var assembly = typeof(YamlConfigurationProvider).Assembly;
            s_ParserType = assembly.GetType("NetEscapades.Configuration.Yaml.YamlConfigurationStreamParser");
            s_ParseMethod = s_ParserType.GetMethod("Parse", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!;
        }

        public YamlConfigurationProviderEx(YamlConfigurationSourceEx source) : base(source)
        {
            m_Source = source;
        }

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

            var parser = Activator.CreateInstance(s_ParserType);

            try
            {
                var invokeResult = s_ParseMethod.Invoke(parser, new object[] { outStream })!;
                Data = (IDictionary<string, string?>)invokeResult;
            }
            catch (YamlException e)
            {
                throw new FormatException($"Could not parse the YAML file: {e.Message}.", e);
            }
        }

        private void PreProcessYaml(ref string yaml)
        {
            if (m_Source.Variables == null)
            {
                return;
            }

            foreach (var variable in m_Source.Variables)
            {
                yaml = yaml.Replace("{{" + variable.Key + "}}", variable.Value);
            }
        }
    }
}
