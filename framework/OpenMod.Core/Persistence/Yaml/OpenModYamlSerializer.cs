using Microsoft.Extensions.Logging;
using OpenMod.API.Ioc;
using OpenMod.API.Persistence;
using System;
using System.Reflection;
using System.Threading.Tasks;
using VYaml.Annotations;
using VYaml.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using Microsoft.Extensions.Options;
using System.Text;

namespace OpenMod.Core.Persistence.Yaml
{
    [ServiceImplementation]
    public class OpenModYamlSerializer : IOpenModSerializer
    {
        private readonly ILogger<OpenModYamlSerializer> m_Logger;
        private readonly ISerializer m_OldYamlSerializer;
        private readonly IDeserializer m_OldYamlDeserializer;

        public OpenModYamlSerializer(
            ILogger<OpenModYamlSerializer> logger,
            IOptions<YamlDataStoreOptions> options)
        {
            m_Logger = logger;

            var serializerBuilder = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeInspector(i => new SerializableIgnoreInspector(i))
                .DisableAliases();

            var deserializerBuilder = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeInspector(i => new SerializableIgnoreInspector(i))
                .IgnoreUnmatchedProperties();

            foreach (var converter in options.Value.Converters)
            {
                serializerBuilder.WithTypeConverter(converter);
                deserializerBuilder.WithTypeConverter(converter);
            }

            m_OldYamlSerializer = serializerBuilder.Build();
            m_OldYamlDeserializer = deserializerBuilder.Build();
        }

        public Task<T?> DeserializeAsync<T>(ReadOnlyMemory<byte> memory)
        {
            if (typeof(T).GetCustomAttribute<YamlObjectAttribute>() == null)
            {
                m_Logger.LogWarning("OpenMod is moving from YamlDotNet to VYaml");
                m_Logger.LogWarning("To migrate you can annotate your class with YamlObjectAttribute or create your own IYamlFormatter");

                var data = UTF8Encoding.UTF8.GetString(memory.Span);
                return Task.FromResult<T?>(m_OldYamlDeserializer.Deserialize<T>(data));
            }

            return Task.FromResult<T?>(YamlSerializer.Deserialize<T>(memory));
        }

        public Task<ReadOnlyMemory<byte>> SerializeAsync<T>(T dataObject)
        {
            if (dataObject == null)
            {
                throw new ArgumentNullException(nameof(dataObject));
            }

            if (typeof(T).GetCustomAttribute<YamlObjectAttribute>() == null)
            {
                m_Logger.LogWarning("OpenMod is moving from YamlDotNet to VYaml");
                m_Logger.LogWarning("To migrate you can annotate your class with YamlObjectAttribute or create your own IYamlFormatter");

                var yamlContent = m_OldYamlSerializer.Serialize(dataObject);
                var encodedData = Encoding.UTF8.GetBytes(yamlContent);
                return Task.FromResult(new ReadOnlyMemory<byte>(encodedData));
            }

            return Task.FromResult(YamlSerializer.Serialize(dataObject));
        }
    }
}
