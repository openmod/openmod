using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using OpenMod.API;
using OpenMod.API.Permissions;
using OpenMod.API.Persistence;
using OpenMod.Core.Configuration;
using OpenMod.Core.Helpers;
using OpenMod.Core.Permissions;
using OpenMod.Core.Persistence;

namespace OpenMod.Core.Users
{
    internal class UsersSchemaGenerator : BackgroundService
    {
        private readonly IOpenModHost m_OpenModHost;
        private readonly IPermissionRoleStore m_PermissionRoleStore;
        private readonly IDataStore m_DataStore;
        private readonly IServiceProvider m_ServiceProvider;

        public UsersSchemaGenerator(
            IOpenModHost openModHost,
            IPermissionRoleStore permissionRoleStore,
            IOpenModDataStoreAccessor dataStoreAccessor)
        {
            m_OpenModHost = openModHost;
            m_PermissionRoleStore = permissionRoleStore;
            m_DataStore = dataStoreAccessor.DataStore;
            m_ServiceProvider = openModHost.LifetimeScope.Resolve<IServiceProvider>();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (m_DataStore is not YamlDataStore)
            {
                return Task.CompletedTask;
            }

            ScheduleSchemaReload();

            m_DataStore.AddChangeWatcher("roles", m_OpenModHost, ScheduleSchemaReload);

            return Task.CompletedTask;
        }

        private void ScheduleSchemaReload()
        {
            var rolesSchemaFile = Path.Combine(m_OpenModHost.WorkingDirectory, SchemaConstants.UsersSchemaPath);

            AsyncHelper.Schedule(
                "Writing " + SchemaConstants.UsersSchemaPath,
                async () => await WriteSchemaAsync(rolesSchemaFile)
            );
        }

        private async Task WriteSchemaAsync(string path)
        {
            var permissions = await PermissionsUtils.GetAllPossiblePermissionsAsync(m_ServiceProvider);
            var permissionsJson = JsonConvert.SerializeObject(permissions);

            var roles = (await m_PermissionRoleStore.GetRolesAsync()).Select(r => r.Id);
            var rolesJson = JsonConvert.SerializeObject(roles);

            var sb = new StringBuilder(c_SchemaTemplate);
            sb.Replace(c_PermissionsPlaceholder, permissionsJson);
            sb.Replace(c_RolesPlaceholder, rolesJson);

            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, sb.ToString());
        }

        private const string c_PermissionsPlaceholder = "<<PERMISSIONS_PLACEHOLDER>>";
        private const string c_RolesPlaceholder = "<<ROLES_PLACEHOLDER>>";

        private const string c_SchemaTemplate = @"
{
    ""$schema"": ""http://json-schema.org/draft-06/schema#"",
    ""$ref"": ""#/definitions/RootObject"",
    ""definitions"": {
        ""RootObject"": {
            ""type"": ""object"",
            ""additionalProperties"": false,
            ""properties"": {
                ""users"": {
                    ""type"": ""array"",
                    ""items"": {
                        ""$ref"": ""#/definitions/User""
                    }
                }
            },
            ""required"": [
                ""users""
            ],
            ""title"": ""Users Configuration""
        },
        ""User"": {
            ""type"": ""object"",
            ""additionalProperties"": false,
            ""properties"": {
                ""id"": {
                    ""type"": ""string"",
                    ""description"": ""The unique identifier of the user.""
                },
                ""type"": {
                    ""type"": ""string""
                },
                ""lastDisplayName"": {
                    ""type"": ""string""
                },
                ""firstSeen"": {
                    ""type"": [""string"", ""null""],
                    ""description"": ""The first time the user has been seen""
                },
                ""lastSeen"": {
                    ""type"": [""string"", ""null""],
                    ""description"": ""The last time the user has been seen""
                },
                ""banInfo"": {
                    ""type"": [""object"", ""null""],
                    ""additionalProperties"": false,
                    ""properties"": {
                        ""expireDate"": {
                            ""type"": ""string""
                        },
                        ""instigatorType"": {
                            ""type"": ""string""
                        },
                        ""instigatorId"": {
                            ""type"": ""string""
                        },
                        ""reason"": {
                            ""type"": ""string""
                        }
                    },
                    ""title"": ""Ban Info""
                },
                ""permissions"": {
                    ""type"": ""array"",
                    ""uniqueItems"": true,
                    ""items"": {
                        ""type"": ""string"",
                        ""enum"": " + c_PermissionsPlaceholder + @"
                    }
                },
                ""roles"": {
                    ""type"": ""array"",
                    ""uniqueItems"": true,
                    ""items"": {
                        ""type"": ""string"",
                        ""enum"": " + c_RolesPlaceholder + @"
                    }
                },
                ""data"": {
                    ""type"": ""object"",
                    ""title"": ""User Data""
                }
            },
            ""required"": [
                ""id"",
                ""type""
            ],
            ""title"": ""User""
        }
    }
}
";
    }
}