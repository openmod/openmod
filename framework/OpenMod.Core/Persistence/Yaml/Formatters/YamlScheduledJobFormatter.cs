using OpenMod.API.Jobs;
using System;
using System.Collections.Generic;
using VYaml.Emitter;
using VYaml.Parser;
using VYaml.Serialization;

namespace OpenMod.Core.Persistence.Yaml.Formatters
{
    public class YamlScheduledJobFormatter : IYamlFormatter<ScheduledJob?>
    {
        private const string c_YamlKeyOfName = "name";
        private const string c_YamlKeyOfArgs = "args";
        private const string c_YamlKeyOfTask = "task";
        private const string c_YamlKeyOfSchedule = "schedule";
        private const string c_YamlKeyOfEnabled = "enabled";

        public ScheduledJob? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
        {
            if (parser.IsNullScalar())
            {
                parser.Read();
                return default;
            }

            string? name = default, task = default, schedule = default;
            Dictionary<string, object?>? args = default;
            bool? enabled = default;

            parser.ReadWithVerify(ParseEventType.MappingStart);

            while (!parser.End && parser.CurrentEventType != ParseEventType.MappingEnd)
            {
                var key = parser.ReadScalarAsString()?.ToLower();
                switch (key)
                {
                    case c_YamlKeyOfName:
                        name = context.DeserializeWithAlias<string>(ref parser);
                        break;

                    case c_YamlKeyOfArgs:
                        args = context.DeserializeWithAlias<Dictionary<string, object?>>(ref parser);
                        break;

                    case c_YamlKeyOfTask:
                        task = context.DeserializeWithAlias<string>(ref parser);
                        break;

                    case c_YamlKeyOfSchedule:
                        schedule = context.DeserializeWithAlias<string>(ref parser);
                        break;

                    case c_YamlKeyOfEnabled:
                        enabled = context.DeserializeWithAlias<bool?>(ref parser);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(key), key, "Fail to parse ScheduledJob");
                }
            }

            parser.ReadWithVerify(ParseEventType.MappingEnd);
            return new ScheduledJob
            {
                Name = name,
                Args = args,
                Task = task,
                Schedule = schedule,
                Enabled = enabled
            };
        }

        public void Serialize(ref Utf8YamlEmitter emitter, ScheduledJob? value, YamlSerializationContext context)
        {
            if (value is null)
            {
                emitter.WriteNull();
                return;
            }

            emitter.BeginMapping();
            emitter.WriteString(c_YamlKeyOfName, ScalarStyle.Plain);
            context.Serialize(ref emitter, value.Name);
            emitter.WriteString(c_YamlKeyOfArgs, ScalarStyle.Plain);
            context.Serialize(ref emitter, value.Args);
            emitter.WriteString(c_YamlKeyOfTask, ScalarStyle.Plain);
            context.Serialize(ref emitter, value.Task);
            emitter.WriteString(c_YamlKeyOfSchedule, ScalarStyle.Plain);
            context.Serialize(ref emitter, value.Schedule);
            emitter.WriteString(c_YamlKeyOfEnabled, ScalarStyle.Plain);
            context.Serialize(ref emitter, value.Enabled);
            emitter.EndMapping();
        }
    }
}
