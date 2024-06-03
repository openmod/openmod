using System.Collections.Generic;
using VYaml.Emitter;
using VYaml.Parser;
using VYaml.Serialization;

namespace OpenMod.NuGet.Persistence
{
    public class YamlHashSetTypeFormatter<T> : IYamlFormatter<HashSet<T>?>, IYamlFormatter
    {
        public static YamlHashSetTypeFormatter<T> Instance = new();
        public void Serialize(ref Utf8YamlEmitter emitter, HashSet<T>? value, YamlSerializationContext context)
        {
            if (value == null)
            {
                emitter.WriteNull();
                return;
            }

            emitter.BeginSequence();
            if (value.Count > 0)
            {
                var formatterWithVerify = context.Resolver.GetFormatterWithVerify<T>();
                foreach (var item in value)
                {
                    formatterWithVerify.Serialize(ref emitter, item, context);
                }
            }

            emitter.EndSequence();
        }

        public HashSet<T>? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
        {
            if (parser.IsNullScalar())
            {
                parser.Read();
                return null;
            }

            parser.ReadWithVerify(ParseEventType.SequenceStart);
            var hashSet = new HashSet<T>();
            IYamlFormatter<T> formatterWithVerify = context.Resolver.GetFormatterWithVerify<T>();
            while (!parser.End && parser.CurrentEventType != ParseEventType.SequenceEnd)
            {
                var item = context.DeserializeWithAlias(formatterWithVerify, ref parser);
                hashSet.Add(item);
            }

            parser.ReadWithVerify(ParseEventType.SequenceEnd);
            return hashSet;
        }
    }
}