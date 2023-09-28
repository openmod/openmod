using System;
using System.IO;
using System.Threading.Tasks;
using Autofac;
using Newtonsoft.Json;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.Core.Configuration;
using OpenMod.Core.Events;
using OpenMod.Core.Helpers;

namespace OpenMod.Core.Permissions
{
    internal class PermissionRolesSchemaGenerator : IEventListener<OpenModInitializedEvent>
    {
        private readonly IServiceProvider m_ServiceProvider;

        public PermissionRolesSchemaGenerator(IOpenModHost openModHost)
        {
            m_ServiceProvider = openModHost.LifetimeScope.Resolve<IServiceProvider>();
        }

        public Task HandleEventAsync(object? sender, OpenModInitializedEvent @event)
        {
            var rolesSchemaFile = Path.Combine(@event.Host.WorkingDirectory, SchemaConstants.RolesSchemaPath);

            AsyncHelper.Schedule(
                "Writing " + SchemaConstants.RolesSchemaPath,
                async () => await WriteSchemaAsync(rolesSchemaFile)
            );

            return Task.CompletedTask;
        }

        private async Task WriteSchemaAsync(string path)
        {
            var permissions = await PermissionsUtils.GetAllPossiblePermissionsAsync(m_ServiceProvider);
            var permissionJson = JsonConvert.SerializeObject(permissions);
            var schema = c_SchemaTemplate.Replace(c_PermissionsPlaceholder, permissionJson);

            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, schema);
        }

        private const string c_PermissionsPlaceholder = "<<PERMISSIONS_PLACEHOLDER>>";

        private const string c_SchemaTemplate = @"
{
    ""$schema"": ""http://json-schema.org/draft-06/schema#"",
    ""$ref"": ""#/definitions/RootObject"",
    ""definitions"": {
        ""RootObject"": {
            ""type"": ""object"",
            ""additionalProperties"": false,
            ""properties"": {
                ""roles"": {
                    ""type"": ""array"",
                    ""items"": {
                        ""$ref"": ""#/definitions/Role""
                    }
                }
            },
            ""required"": [
                ""roles""
            ],
            ""title"": ""Roles Configuration""
        },
        ""Role"": {
            ""type"": ""object"",
            ""additionalProperties"": false,
            ""properties"": {
                ""id"": {
                    ""type"": ""string"",
                    ""description"": ""The unique identifier of the role.""
                },
                ""priority"": {
                    ""type"": ""integer"",
                    ""description"": ""In case of conflicting permissions, this attribute will define which role gets preferred.""
                },
                ""parents"": {
                    ""type"": ""array"",
                    ""uniqueItems"": true,
                    ""items"": {
                        ""type"": ""string""
                    },
                    ""description"": ""The parent roles, whose permissions are inherited.""
                },
                ""permissions"": {
                    ""type"": ""array"",
                    ""uniqueItems"": true,
                    ""items"": {
                        ""type"": ""string"",
                        ""enum"": " + c_PermissionsPlaceholder + @"
                    },
                    ""description"": ""List of permissions the role has.""
                },
                ""displayName"": {
                    ""type"": ""string"",
                    ""description"": ""Human-readable name of the role.""
                },
                ""data"": {
                    ""type"": ""object"",
                    ""title"": ""Data"",
                    ""description"": ""Data that can be attached to the role by plugins.""
                },
                ""isAutoAssigned"": {
                    ""type"": ""boolean"",
                    ""description"": ""Automatically assigns the role to new users. Does not assign to existing users.""
                }
            },
            ""required"": [
                ""id"",
                ""isAutoAssigned"",
                ""priority""
            ],
            ""title"": ""Role""
        }
    }
}
";
    }
}