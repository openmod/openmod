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
using System.Linq;

namespace OpenMod.Core.Persistence.Yaml
{
    [ServiceImplementation]
    public class OpenModYamlSerializer : IOpenModSerializer
    {
        private readonly ILogger<OpenModYamlSerializer> m_Logger;
        private readonly ISerializer m_OldYamlSerializer;
        private readonly IDeserializer m_OldYamlDeserializer;
        private readonly YamlSerializerOptions m_YamlOptions;

        public OpenModYamlSerializer(
            ILogger<OpenModYamlSerializer> logger,
            IOptions<YamlDataStoreOptions> options)
        {
            m_Logger = logger;

#pragma warning disable CS0618 // Obsolete
            var serializerBuilder = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeInspector(i => new SerializableIgnoreInspector(i))
                .DisableAliases();

            var deserializerBuilder = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeInspector(i => new SerializableIgnoreInspector(i))
                .IgnoreUnmatchedProperties();
#pragma warning restore CS0618

            foreach (var converter in options.Value.Converters)
            {
                serializerBuilder.WithTypeConverter(converter);
                deserializerBuilder.WithTypeConverter(converter);
            }

            m_OldYamlSerializer = serializerBuilder.Build();
            m_OldYamlDeserializer = deserializerBuilder.Build();

            m_YamlOptions = new YamlSerializerOptions
            {
                Resolver = CompositeResolver.Create(options.Value.Formatters, options.Value.FormatterResolvers.Append(StandardResolver.Instance))
            };
        }

        public Task<T?> DeserializeAsync<T>(ReadOnlyMemory<byte> memory)
        {
            if (memory.IsEmpty)
            {
                return Task.FromResult<T?>(default);
            }

            if (typeof(T).GetCustomAttribute<YamlObjectAttribute>() == null)
            {
                m_Logger.LogWarning("OpenMod is moving from YamlDotNet to VYaml");
                m_Logger.LogWarning($"Annotate your class({typeof(T)}) with YamlObjectAttribute or create your own IYamlFormatter");

                var data = UTF8Encoding.UTF8.GetString(memory.Span);
                return Task.FromResult<T?>(m_OldYamlDeserializer.Deserialize<T>(data));
            }

            return Task.FromResult<T?>(YamlSerializer.Deserialize<T>(memory, m_YamlOptions));
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
                m_Logger.LogWarning($"Annotate your class({typeof(T)} with YamlObjectAttribute or create your own IYamlFormatter");

                var yamlContent = m_OldYamlSerializer.Serialize(dataObject);
                var encodedData = Encoding.UTF8.GetBytes(yamlContent);
                return Task.FromResult(new ReadOnlyMemory<byte>(encodedData));
            }

            return Task.FromResult(YamlSerializer.Serialize(dataObject, m_YamlOptions));
        }
    }
}
