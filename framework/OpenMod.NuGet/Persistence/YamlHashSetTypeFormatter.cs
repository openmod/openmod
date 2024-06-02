using System.Collections.Generic;
using VYaml.Emitter;
using VYaml.Parser;
using VYaml.Serialization;

namespace OpenMod.NuGet.Persistence
{
    public class SerializedNuGetPackageTypeFormatter : IYamlFormatter<HashSet<SerializedNuGetPackage>?>, IYamlFormatter
    {
        public static SerializedNuGetPackageTypeFormatter Instance = new();
        public void Serialize(ref Utf8YamlEmitter emitter, HashSet<SerializedNuGetPackage>? value, YamlSerializationContext context)
        {
            if (value == null)
            {
                emitter.WriteNull();
                return;
            }

            emitter.BeginSequence();
            if (value.Count > 0)
            {
                var formatterWithVerify = context.Resolver.GetFormatterWithVerify<SerializedNuGetPackage>();
                foreach (var item in value)
                {
                    formatterWithVerify.Serialize(ref emitter, item, context);
                }
            }

            emitter.EndSequence();
        }

        public HashSet<SerializedNuGetPackage>? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
        {
            if (parser.IsNullScalar())
            {
                parser.Read();
                return null;
            }

            parser.ReadWithVerify(ParseEventType.SequenceStart);
            var hashSet = new HashSet<SerializedNuGetPackage>();
            IYamlFormatter<SerializedNuGetPackage> formatterWithVerify = context.Resolver.GetFormatterWithVerify<SerializedNuGetPackage>();
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