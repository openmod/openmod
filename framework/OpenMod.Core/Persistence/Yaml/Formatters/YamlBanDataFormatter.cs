using OpenMod.API.Users;
using System;
using VYaml.Emitter;
using VYaml.Parser;
using VYaml.Serialization;

namespace OpenMod.Core.Persistence.Yaml.Formatters
{
    public class YamlBanDataFormatter : IYamlFormatter<BanData?>
    {
        private const string c_YamlKeyOfExpireDate = "expiredate";
        private const string c_YamlKeyOfInstigatorType = "instigatortype";
        private const string c_YamlKeyOfInstigatorId = "instigatorid";
        private const string c_YamlKeyOfReason = "reason";

        public BanData? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
        {
            if (parser.IsNullScalar())
            {
                parser.Read();
                return default;
            }

            DateTime? expireDate = null;
            string? instigatorType = default, instigatorId = default, reason = default;

            parser.ReadWithVerify(ParseEventType.MappingStart);

            while (!parser.End && parser.CurrentEventType != ParseEventType.MappingEnd)
            {
                var key = parser.ReadScalarAsString()?.ToLower();
                switch (key)
                {
                    case c_YamlKeyOfExpireDate:
                        expireDate = context.DeserializeWithAlias<DateTime?>(ref parser);
                        break;

                    case c_YamlKeyOfInstigatorType:
                        instigatorType = context.DeserializeWithAlias<string?>(ref parser);
                        break;

                    case c_YamlKeyOfInstigatorId:
                        instigatorId = context.DeserializeWithAlias<string?>(ref parser);
                        break;

                    case c_YamlKeyOfReason:
                        reason = context.DeserializeWithAlias<string?>(ref parser);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(key), key, "Fail to parse BanData");
                }
            }

            parser.ReadWithVerify(ParseEventType.MappingEnd);
            return new BanData
            {
                ExpireDate = expireDate,
                InstigatorType = instigatorType,
                InstigatorId = instigatorId,
                Reason = reason
            };
        }

        public void Serialize(ref Utf8YamlEmitter emitter, BanData? value, YamlSerializationContext context)
        {
            if (value is null)
            {
                emitter.WriteNull();
                return;
            }

            emitter.BeginMapping();
            emitter.WriteString(c_YamlKeyOfExpireDate, ScalarStyle.Plain);
            context.Serialize(ref emitter, value.ExpireDate);
            emitter.WriteString(c_YamlKeyOfInstigatorType, ScalarStyle.Plain);
            context.Serialize(ref emitter, value.InstigatorType);
            emitter.WriteString(c_YamlKeyOfInstigatorId, ScalarStyle.Plain);
            context.Serialize(ref emitter, value.InstigatorId);
            emitter.WriteString(c_YamlKeyOfReason, ScalarStyle.Plain);
            context.Serialize(ref emitter, value.Reason);
            emitter.EndMapping();
        }
    }
}
