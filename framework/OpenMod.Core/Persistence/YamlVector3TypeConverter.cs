using OpenMod.API;
using System;
using System.Numerics;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace OpenMod.Core.Persistence;

[OpenModInternal]
public class YamlVector3TypeConverter : IYamlTypeConverter
{
    private const string c_YamlKeyOfX = "x";

    private const string c_YamlKeyOfY = "y";

    private const string c_YamlKeyOfZ = "z";

    public bool Accepts(Type type)
    {
        return type == typeof(Vector3);
    }

    public object? ReadYaml(IParser parser, Type type)
    {
        parser.Consume<MappingStart>();

        float x = 0, y = 0, z = 0;

        while (!parser.TryConsume<MappingEnd>(out _))
        {
            var key = parser.Consume<Scalar>();
            var value = parser.Consume<Scalar>();

            switch (key.Value)
            {
                case c_YamlKeyOfX:
                    x = float.Parse(value.Value, YamlFormatter.Default.NumberFormat);
                    break;
                case c_YamlKeyOfY:
                    y = float.Parse(value.Value, YamlFormatter.Default.NumberFormat);
                    break;
                case c_YamlKeyOfZ:
                    z = float.Parse(value.Value, YamlFormatter.Default.NumberFormat);
                    break;
            }
        }

        return new Vector3(x, y, z);
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type)
    {
        if (value == null)
        {
            return;
        }

        var vector = (Vector3)value;

        emitter.Emit(new MappingStart());

        emitter.Emit(new Scalar(c_YamlKeyOfX));
        emitter.Emit(new Scalar(vector.X.ToString()));

        emitter.Emit(new Scalar(c_YamlKeyOfY));
        emitter.Emit(new Scalar(vector.Y.ToString()));

        emitter.Emit(new Scalar(c_YamlKeyOfZ));
        emitter.Emit(new Scalar(vector.Z.ToString()));

        emitter.Emit(new MappingEnd());
    }
}