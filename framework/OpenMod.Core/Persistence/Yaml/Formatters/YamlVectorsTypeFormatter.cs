using OpenMod.API;
using System;
using System.Numerics;
using VYaml.Emitter;
using VYaml.Parser;
using VYaml.Serialization;

namespace OpenMod.Core.Persistence.Yaml.Formatters
{
    [OpenModInternal]
    public class YamlVectorsTypeFormatter : IYamlFormatter<Vector2?>, IYamlFormatter<Vector3?>, IYamlFormatter<Vector4?>, IYamlFormatter<Quaternion?>
    {
        private const string c_YamlKeyOfX = "x";
        private const string c_YamlKeyOfY = "y";
        private const string c_YamlKeyOfZ = "z";
        private const string c_YamlKeyOfW = "w";

        #region Vector2
        Vector2? IYamlFormatter<Vector2?>.Deserialize(ref YamlParser parser, YamlDeserializationContext context)
        {
            if (parser.IsNullScalar())
            {
                parser.Read();
                return default;
            }

            float x = 0, y = 0;
            parser.ReadWithVerify(ParseEventType.MappingStart);

            while (!parser.End && parser.CurrentEventType != ParseEventType.MappingEnd)
            {
                var key = parser.ReadScalarAsString()?.ToLower();
                var value = context.DeserializeWithAlias<float>(ref parser);

                switch (key)
                {
                    case c_YamlKeyOfX:
                        x = value;
                        break;

                    case c_YamlKeyOfY:
                        y = value;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(key), key, "Fail to parse Vector2");
                }
            }

            parser.ReadWithVerify(ParseEventType.MappingEnd);
            return new Vector2(x, y);
        }

        public void Serialize(ref Utf8YamlEmitter emitter, Vector2? value, YamlSerializationContext context)
        {
            if (!value.HasValue)
            {
                emitter.WriteNull();
                return;
            }

            emitter.BeginMapping();
            emitter.WriteString(c_YamlKeyOfX, ScalarStyle.Plain);
            context.Serialize(ref emitter, value.Value.X);
            emitter.WriteString(c_YamlKeyOfY, ScalarStyle.Plain);
            context.Serialize(ref emitter, value.Value.Y);
            emitter.EndMapping();
        }
        #endregion Vector2

        #region Vector3
        Vector3? IYamlFormatter<Vector3?>.Deserialize(ref YamlParser parser, YamlDeserializationContext context)
        {
            if (parser.IsNullScalar())
            {
                parser.Read();
                return default;
            }

            float x = 0, y = 0, z = 0;
            parser.ReadWithVerify(ParseEventType.MappingStart);

            while (!parser.End && parser.CurrentEventType != ParseEventType.MappingEnd)
            {
                var key = parser.ReadScalarAsString()?.ToLower();
                var value = context.DeserializeWithAlias<float>(ref parser);

                switch (key)
                {
                    case c_YamlKeyOfX:
                        x = value;
                        break;

                    case c_YamlKeyOfY:
                        y = value;
                        break;

                    case c_YamlKeyOfZ:
                        z = value;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(key), key, "Fail to parse Vector3");
                }
            }

            parser.ReadWithVerify(ParseEventType.MappingEnd);
            return new Vector3(x, y, z);
        }

        public void Serialize(ref Utf8YamlEmitter emitter, Vector3? value, YamlSerializationContext context)
        {
            if (!value.HasValue)
            {
                emitter.WriteNull();
                return;
            }

            emitter.BeginMapping();
            emitter.WriteString(c_YamlKeyOfX, ScalarStyle.Plain);
            context.Serialize(ref emitter, value.Value.X);
            emitter.WriteString(c_YamlKeyOfY, ScalarStyle.Plain);
            context.Serialize(ref emitter, value.Value.Y);
            emitter.WriteString(c_YamlKeyOfZ, ScalarStyle.Plain);
            context.Serialize(ref emitter, value.Value.Z);
            emitter.EndMapping();
        }
        #endregion Vector3

        #region Vector4
        Vector4? IYamlFormatter<Vector4?>.Deserialize(ref YamlParser parser, YamlDeserializationContext context)
        {
            if (parser.IsNullScalar())
            {
                parser.Read();
                return default;
            }

            float x = 0, y = 0, z = 0, w = 0;
            parser.ReadWithVerify(ParseEventType.MappingStart);

            while (!parser.End && parser.CurrentEventType != ParseEventType.MappingEnd)
            {
                var key = parser.ReadScalarAsString()?.ToLower();
                var value = context.DeserializeWithAlias<float>(ref parser);

                switch (key)
                {
                    case c_YamlKeyOfX:
                        x = value;
                        break;

                    case c_YamlKeyOfY:
                        y = value;
                        break;

                    case c_YamlKeyOfZ:
                        z = value;
                        break;

                    case c_YamlKeyOfW:
                        w = value;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(key), key, "Fail to parse Vector4");
                }
            }

            parser.ReadWithVerify(ParseEventType.MappingEnd);
            return new Vector4(x, y, z, w);
        }

        public void Serialize(ref Utf8YamlEmitter emitter, Vector4? value, YamlSerializationContext context)
        {
            if (!value.HasValue)
            {
                emitter.WriteNull();
                return;
            }

            emitter.BeginMapping();
            emitter.WriteString(c_YamlKeyOfX, ScalarStyle.Plain);
            context.Serialize(ref emitter, value.Value.X);
            emitter.WriteString(c_YamlKeyOfY, ScalarStyle.Plain);
            context.Serialize(ref emitter, value.Value.Y);
            emitter.WriteString(c_YamlKeyOfZ, ScalarStyle.Plain);
            context.Serialize(ref emitter, value.Value.Z);
            emitter.WriteString(c_YamlKeyOfW, ScalarStyle.Plain);
            context.Serialize(ref emitter, value.Value.W);
            emitter.EndMapping();
        }
        #endregion Vector4

        #region Quaternion
        Quaternion? IYamlFormatter<Quaternion?>.Deserialize(ref YamlParser parser, YamlDeserializationContext context)
        {
            if (parser.IsNullScalar())
            {
                parser.Read();
                return default;
            }

            float x = 0, y = 0, z = 0, w = 0;
            parser.ReadWithVerify(ParseEventType.MappingStart);

            while (!parser.End && parser.CurrentEventType != ParseEventType.MappingEnd)
            {
                var key = parser.ReadScalarAsString()?.ToLower();
                var value = context.DeserializeWithAlias<float>(ref parser);

                switch (key)
                {
                    case c_YamlKeyOfX:
                        x = value;
                        break;

                    case c_YamlKeyOfY:
                        y = value;
                        break;

                    case c_YamlKeyOfZ:
                        z = value;
                        break;

                    case c_YamlKeyOfW:
                        w = value;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(key), key, "Fail to parse Quaternion");
                }
            }

            parser.ReadWithVerify(ParseEventType.MappingEnd);
            return new Quaternion(x, y, z, w);
        }

        public void Serialize(ref Utf8YamlEmitter emitter, Quaternion? value, YamlSerializationContext context)
        {
            if (!value.HasValue)
            {
                emitter.WriteNull();
                return;
            }

            emitter.BeginMapping();
            emitter.WriteString(c_YamlKeyOfX, ScalarStyle.Plain);
            context.Serialize(ref emitter, value.Value.X);
            emitter.WriteString(c_YamlKeyOfY, ScalarStyle.Plain);
            context.Serialize(ref emitter, value.Value.Y);
            emitter.WriteString(c_YamlKeyOfZ, ScalarStyle.Plain);
            context.Serialize(ref emitter, value.Value.Z);
            emitter.WriteString(c_YamlKeyOfW, ScalarStyle.Plain);
            context.Serialize(ref emitter, value.Value.W);
            emitter.EndMapping();
        }
        #endregion Quaternion
    }
}