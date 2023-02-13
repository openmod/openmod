using System;
using System.Collections.Generic;
using System.Globalization;
using OpenMod.API.Users;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.Schemas;

namespace OpenMod.Core.Users
{
    [Serializable]
    public sealed class UsersData : IYamlConvertible
    {
        public List<UserData>? Users { get; set; }

        public void Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
        {
            parser.Consume<MappingStart>();
            parser.Consume<Scalar>();

            if (parser.Accept<SequenceStart>(out _))
            {
                Users = nestedObjectDeserializer(typeof(List<UserData>)) as List<UserData>;
            }
            else
            {
                parser.Consume<Scalar>();
            }

            parser.Consume<MappingEnd>();
        }

        public void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
        {
            var nullScalar = new Scalar(AnchorName.Empty, JsonSchema.Tags.Null, string.Empty, ScalarStyle.Plain, true, false);
            var mappingStart = new MappingStart(AnchorName.Empty, TagName.Empty, false, MappingStyle.Any);
            var mappingEnd = new MappingEnd();

            emitter.Emit(mappingStart);
            emitter.Emit(new Scalar(AnchorName.Empty, FailsafeSchema.Tags.Str, "users", ScalarStyle.Plain, true, false));
            if (Users == null)
            {
                emitter.Emit(nullScalar);
                emitter.Emit(mappingEnd);
                return;
            }

            emitter.Emit(new SequenceStart(AnchorName.Empty, TagName.Empty, false, SequenceStyle.Any));
            foreach (var user in Users)
            {
                if (user == null)
                    continue;

                emitter.Emit(mappingStart);

                GetTagAndRenderedValue(user.Id, out var tag, out var value);
                emitter.Emit(new Scalar(AnchorName.Empty, FailsafeSchema.Tags.Str, "id", ScalarStyle.Plain, true, false));
                emitter.Emit(new Scalar(AnchorName.Empty, tag, value, ScalarStyle.Any, true, false));

                GetTagAndRenderedValue(user.Type, out tag, out value);
                emitter.Emit(new Scalar(AnchorName.Empty, FailsafeSchema.Tags.Str, "type", ScalarStyle.Plain, true, false));
                emitter.Emit(new Scalar(AnchorName.Empty, tag, value, ScalarStyle.Any, true, false));

                GetTagAndRenderedValue(user.LastDisplayName, out tag, out value);
                emitter.Emit(new Scalar(AnchorName.Empty, FailsafeSchema.Tags.Str, "lastDisplayName", ScalarStyle.Plain, true, false));
                emitter.Emit(new Scalar(AnchorName.Empty, tag, value, ScalarStyle.Any, true, false));

                GetTagAndRenderedValue(user.FirstSeen, out tag, out value);
                emitter.Emit(new Scalar(AnchorName.Empty, FailsafeSchema.Tags.Str, "firstSeen", ScalarStyle.Plain, true, false));
                emitter.Emit(new Scalar(AnchorName.Empty, tag, value, ScalarStyle.Any, true, false));

                GetTagAndRenderedValue(user.LastSeen, out tag, out value);
                emitter.Emit(new Scalar(AnchorName.Empty, FailsafeSchema.Tags.Str, "lastSeen", ScalarStyle.Plain, true, false));
                emitter.Emit(new Scalar(AnchorName.Empty, tag, value, ScalarStyle.Any, true, false));

                emitter.Emit(new Scalar(AnchorName.Empty, FailsafeSchema.Tags.Str, "banInfo", ScalarStyle.Plain, true, false));
                if (user.BanInfo != null)
                {
                    EmitBanData(emitter, user.BanInfo);
                }
                else
                {
                    emitter.Emit(nullScalar);
                }

                emitter.Emit(new Scalar(AnchorName.Empty, FailsafeSchema.Tags.Str, "permissions", ScalarStyle.Plain, true, false));
                if (user.Permissions != null)
                {
                    EmitHashSet(emitter, user.Permissions);
                }
                else
                {
                    emitter.Emit(nullScalar);
                }

                emitter.Emit(new Scalar(AnchorName.Empty, FailsafeSchema.Tags.Str, "roles", ScalarStyle.Plain, true, false));
                if (user.Roles != null)
                {
                    EmitHashSet(emitter, user.Roles);
                }
                else
                {
                    emitter.Emit(nullScalar);
                }

                emitter.Emit(new Scalar(AnchorName.Empty, FailsafeSchema.Tags.Str, "data", ScalarStyle.Plain, true, false));
                if (user.Data != null)
                {
                    EmitDictionary(emitter, user.Data, nestedObjectSerializer);
                }
                else
                {
                    emitter.Emit(nullScalar);
                }

                emitter.Emit(mappingEnd);
            }
            emitter.Emit(new SequenceEnd());
            emitter.Emit(mappingEnd);
        }

        private static void GetTagAndRenderedValue(string? value, out TagName tagName, out string renderedValue)
        {
            tagName = value is null ? JsonSchema.Tags.Null : FailsafeSchema.Tags.Str;
            renderedValue = value ?? string.Empty;
        }

        private static void GetTagAndRenderedValue(DateTime? value, out TagName tagName, out string renderedValue)
        {
            if (value.HasValue)
            {
                tagName = DefaultSchema.Tags.Timestamp;
                renderedValue = value.Value.ToString("O", CultureInfo.InvariantCulture);
                return;
            }

            tagName = JsonSchema.Tags.Null;
            renderedValue = string.Empty;
        }

        private static void EmitBanData(IEmitter emitter, BanData banData)
        {
            emitter.Emit(new MappingStart(AnchorName.Empty, TagName.Empty, true, MappingStyle.Any));

            GetTagAndRenderedValue(banData.ExpireDate, out var tag, out var value);
            emitter.Emit(new Scalar(AnchorName.Empty, FailsafeSchema.Tags.Str, "expireDate", ScalarStyle.Plain, true, false));
            emitter.Emit(new Scalar(AnchorName.Empty, tag, value, ScalarStyle.Any, true, false));

            GetTagAndRenderedValue(banData.InstigatorType, out tag, out value);
            emitter.Emit(new Scalar(AnchorName.Empty, FailsafeSchema.Tags.Str, "instigatorType", ScalarStyle.Plain, true, false));
            emitter.Emit(new Scalar(AnchorName.Empty, tag, value, ScalarStyle.Any, true, false));

            GetTagAndRenderedValue(banData.InstigatorId, out tag, out value);
            emitter.Emit(new Scalar(AnchorName.Empty, FailsafeSchema.Tags.Str, "instigatorId", ScalarStyle.Plain, true, false));
            emitter.Emit(new Scalar(AnchorName.Empty, tag, value, ScalarStyle.Any, true, false));

            GetTagAndRenderedValue(banData.Reason, out tag, out value);
            emitter.Emit(new Scalar(AnchorName.Empty, FailsafeSchema.Tags.Str, "reason", ScalarStyle.Plain, true, false));
            emitter.Emit(new Scalar(AnchorName.Empty, tag, value, ScalarStyle.Any, true, false));

            emitter.Emit(new MappingEnd());
        }

        private static void EmitHashSet(IEmitter emitter, HashSet<string>? set)
        {
            emitter.Emit(new SequenceStart(AnchorName.Empty, TagName.Empty, true, SequenceStyle.Any));

            if (set is not null)
            {
                foreach (var value in set)
                {
                    if (value is null)
                    {
                        continue;
                    }

                    emitter.Emit(new Scalar(AnchorName.Empty, FailsafeSchema.Tags.Str, value, ScalarStyle.Plain, true, false));
                }
            }

            emitter.Emit(new SequenceEnd());
        }

        private static void EmitDictionary(IEmitter emitter, Dictionary<string, object?>? dict, ObjectSerializer objectSerializer)
        {
            emitter.Emit(new MappingStart(AnchorName.Empty, TagName.Empty, false, MappingStyle.Any));

            if (dict is not null)
            {
                foreach (var kv in dict)
                {
                    emitter.Emit(new Scalar(AnchorName.Empty, FailsafeSchema.Tags.Str, kv.Key, ScalarStyle.Plain, true, false));
                    objectSerializer(kv.Value);
                }
            }

            emitter.Emit(new MappingEnd());
        }
    }
}