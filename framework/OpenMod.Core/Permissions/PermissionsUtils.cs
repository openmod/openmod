using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.API.Permissions;
using OpenMod.API.Plugins;
#if !NETSTANDARD2_1_OR_GREATER
using MoreLinq;
#endif

namespace OpenMod.Core.Permissions
{
    internal static class PermissionsUtils
    {
        public static async Task<ISet<string>> GetAllPossiblePermissionsAsync(IServiceProvider serviceProvider)
        {
            var permissionRegistry = serviceProvider.GetRequiredService<IPermissionRegistry>();
            var openModHost = serviceProvider.GetRequiredService<IOpenModHost>();
            var pluginActivator = serviceProvider.GetRequiredService<IPluginActivator>();
            var commandStore = serviceProvider.GetRequiredService<ICommandStore>();
            var commandPermissionBuilder = serviceProvider.GetRequiredService<ICommandPermissionBuilder>();

            var openModComponents = new IOpenModComponent[] { openModHost }
                .Concat(pluginActivator.ActivatedPlugins)
                .ToArray();

            var permissionsOfAllCommands = await GetPermissionsOfAllCommandsAsync(commandStore, commandPermissionBuilder);
            var permissionsOfAllComponents = GetPermissionsOfAllComponents(permissionRegistry, openModComponents);

            var allPermissions = permissionsOfAllComponents
                .Union(permissionsOfAllCommands)
                .ToArray();

            return allPermissions
                .Union(allPermissions.Select(p => '!' + p))
                .ToHashSet();
        }

        private static async Task<ISet<string>> GetPermissionsOfAllCommandsAsync(
            ICommandStore commandStore,
            ICommandPermissionBuilder commandPermissionBuilder)
        {
            var commands = await commandStore.GetCommandsAsync();

            return commands
                .SelectMany(c =>
                {
                    var permission = commandPermissionBuilder.GetPermission(c, commands);

                    var permissionRegistrations = c.PermissionRegistrations?
                        .Select(r => permission + '.' + r.Permission) ?? Enumerable.Empty<string>();

                    // ReSharper disable once InvokeAsExtensionMethod
                    return MoreLinq.MoreEnumerable.Append(permissionRegistrations, permission);
                })
                .SelectMany(DefaultPermissionCheckProvider.BuildPermissionTree)
                .ToHashSet();
        }

        private static ISet<string> GetPermissionsOfAllComponents(
            IPermissionRegistry permissionRegistry,
            IReadOnlyCollection<IOpenModComponent> openModComponents)
        {
            return openModComponents
                .SelectMany(c => permissionRegistry
                    .GetPermissions(c)
                    .Select(r => c.OpenModComponentId + ':' + r.Permission))
                .SelectMany(DefaultPermissionCheckProvider.BuildPermissionTree)
                .ToHashSet();
        }
    }
}
