using System;
using OpenMod.API;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace OpenMod.Core.Persistence
{
    /// Fixes an issue introduced in YamlDotNet 9.1
    /// https://github.com/aaubry/YamlDotNet/issues/544
    [OpenModInternal]
    public class YamlNullableEnumTypeConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return Nullable.GetUnderlyingType(type)?.IsEnum ?? false;
        }

        public object? ReadYaml(IParser parser, Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? throw new ArgumentException("Expected nullable enum type for ReadYaml");
            var scalar = parser.Consume<Scalar>();

            if (string.IsNullOrWhiteSpace(scalar.Value))
            {
                return null;
            }

            try
            {
                return Enum.Parse(type, scalar.Value);
            }
            catch(Exception ex)
            {
                throw new Exception($"Invalid value: \"{scalar.Value}\" for {type.Name}", ex);
            }
        }

        public void WriteYaml(IEmitter emitter, object? value, Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? throw new ArgumentException("Expected nullable enum type for WriteYaml");

            if (value != null)
            {
                var toWrite = Enum.GetName(type, value) ?? throw new InvalidOperationException($"Invalid value {value} for enum: {type}");
                emitter.Emit(new Scalar(null, null, toWrite, ScalarStyle.Any, true, false));
            }
        }
    }
}