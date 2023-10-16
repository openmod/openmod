using OpenMod.API;
using System;
using System.Numerics;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace OpenMod.Core.Persistence;

[OpenModInternal]
public class Vector3YamlConverter : IYamlTypeConverter
{
    public bool Accepts(Type type)
    {
        return type == typeof(Vector3);
    }

    public object ReadYaml(IParser parser, Type type)
    {
        if (!parser.TryConsume<MappingStart>(out _))
        {
            throw new InvalidOperationException("Invalid Vector3 format in YAML.");
        }

        float x = 0, y = 0, z = 0;

        while (!parser.TryConsume<MappingEnd>(out _))
        {
            var key = parser.Consume<Scalar>();
            var value = parser.Consume<Scalar>();

            switch (key.Value)
            {
                case "X":
                    x = float.Parse(value.Value);
                    break;
                case "Y":
                    y = float.Parse(value.Value);
                    break;
                case "Z":
                    z = float.Parse(value.Value);
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
        emitter.Emit(new Scalar("X"));
        emitter.Emit(new Scalar(vector.X.ToString()));
        emitter.Emit(new Scalar("Y"));
        emitter.Emit(new Scalar(vector.Y.ToString()));
        emitter.Emit(new Scalar("Z"));
        emitter.Emit(new Scalar(vector.Z.ToString()));
        emitter.Emit(new MappingEnd());
    }
}