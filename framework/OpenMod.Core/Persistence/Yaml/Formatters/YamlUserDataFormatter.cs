using OpenMod.API.Users;
using System;
using System.Collections.Generic;
using VYaml.Emitter;
using VYaml.Parser;
using VYaml.Serialization;

namespace OpenMod.Core.Persistence.Yaml.Formatters
{
    public class YamlUserDataFormatter : IYamlFormatter<UserData?>
    {
        private const string c_YamlKeyOfId = "id";
        private const string c_YamlKeyOfType = "type";
        private const string c_YamlKeyOfLastDisplayName = "lastdisplayname";
        private const string c_YamlKeyOfFirstSeen = "firstseen";
        private const string c_YamlKeyOfLastSeen = "lastseen";
        private const string c_YamlKeyOfBanInfo = "baninfo";
        private const string c_YamlKeyOfPermissions = "permissions";
        private const string c_YamlKeyOfRoles = "roles";
        private const string c_YamlKeyOfData = "data";

        public UserData? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
        {
            if (parser.IsNullScalar())
            {
                parser.Read();
                return default;
            }

            string? id = default, type = default, lastDisplayName = default;
            DateTime? firstSeen = default, lastSeen = default;
            BanData? banInfo = default;
            HashSet<string>? permissions = default, roles = default;
            Dictionary<string, object?>? data = default;

            parser.ReadWithVerify(ParseEventType.MappingStart);

            while (!parser.End && parser.CurrentEventType != ParseEventType.MappingEnd)
            {
                var key = parser.ReadScalarAsString()?.ToLower();
                switch (key)
                {
                    case c_YamlKeyOfId:
                        id = context.DeserializeWithAlias<string?>(ref parser);
                        break;

                    case c_YamlKeyOfType:
                        type = context.DeserializeWithAlias<string?>(ref parser);
                        break;

                    case c_YamlKeyOfLastDisplayName:
                        lastDisplayName = context.DeserializeWithAlias<string?>(ref parser);
                        break;

                    case c_YamlKeyOfFirstSeen:
                        firstSeen = context.DeserializeWithAlias<DateTime?>(ref parser);
                        break;

                    case c_YamlKeyOfLastSeen:
                        lastSeen = context.DeserializeWithAlias<DateTime?>(ref parser);
                        break;

                    case c_YamlKeyOfBanInfo:
                        banInfo = context.DeserializeWithAlias<BanData?>(ref parser);
                        break;

                    case c_YamlKeyOfPermissions:
                        permissions = context.DeserializeWithAlias<HashSet<string>?>(ref parser);
                        break;

                    case c_YamlKeyOfRoles:
                        roles = context.DeserializeWithAlias<HashSet<string>?>(ref parser);
                        break;

                    case c_YamlKeyOfData:
                        data = context.DeserializeWithAlias<Dictionary<string, object?>?>(ref parser);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(key), key, "Fail to parse UserData");
                }
            }

            parser.ReadWithVerify(ParseEventType.MappingEnd);
            return new UserData
            {
                Id = id,
                Type = type,
                LastDisplayName = lastDisplayName,
                FirstSeen = firstSeen,
                LastSeen = lastSeen,
                BanInfo = banInfo,
                Permissions = permissions,
                Roles = roles,
                Data = data
            };
        }

        public void Serialize(ref Utf8YamlEmitter emitter, UserData? value, YamlSerializationContext context)
        {
            if (value is null)
            {
                emitter.WriteNull();
                return;
            }

            emitter.BeginMapping();
            emitter.WriteString(c_YamlKeyOfId, ScalarStyle.Plain);
            context.Serialize(ref emitter, value.Id);
            emitter.WriteString(c_YamlKeyOfType, ScalarStyle.Plain);
            context.Serialize(ref emitter, value.Type);
            emitter.WriteString(c_YamlKeyOfLastDisplayName, ScalarStyle.Plain);
            context.Serialize(ref emitter, value.LastDisplayName);
            emitter.WriteString(c_YamlKeyOfFirstSeen, ScalarStyle.Plain);
            context.Serialize(ref emitter, value.FirstSeen);
            emitter.WriteString(c_YamlKeyOfLastSeen, ScalarStyle.Plain);
            context.Serialize(ref emitter, value.LastSeen);
            emitter.WriteString(c_YamlKeyOfBanInfo, ScalarStyle.Plain);
            context.Serialize(ref emitter, value.BanInfo);
            emitter.WriteString(c_YamlKeyOfPermissions, ScalarStyle.Plain);
            context.Serialize(ref emitter, value.Permissions);
            emitter.WriteString(c_YamlKeyOfRoles, ScalarStyle.Plain);
            context.Serialize(ref emitter, value.Roles);
            emitter.WriteString(c_YamlKeyOfData, ScalarStyle.Plain);
            context.Serialize(ref emitter, value.Data);
            emitter.EndMapping();
        }
    }
}
