using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.API.Permissions;
using OpenMod.API.Plugins;

namespace OpenMod.Core.Plugins
{
    public class PluginHelpWriter
    {
        private readonly IOpenModPlugin m_Plugin;
        private readonly ICommandStore m_CommandStore;
        private readonly ICommandContextBuilder m_CommandContextBuilder;
        private readonly ICommandPermissionBuilder m_PermissionBuilder;
        private readonly IPermissionRegistry m_PermissionRegistry;
        private readonly List<IPermissionRegistration> m_PrintedCommandPermissions;

        public PluginHelpWriter(
            ICommandPermissionBuilder permissionBuilder,
            IPermissionRegistry permissionRegistry,
            IOpenModPlugin plugin,
            ICommandStore commandStore,
            ICommandContextBuilder commandContextBuilder)
        {
            m_PermissionBuilder = permissionBuilder;
            m_PermissionRegistry = permissionRegistry;
            m_Plugin = plugin;
            m_CommandStore = commandStore;
            m_CommandContextBuilder = commandContextBuilder;
            m_PrintedCommandPermissions = new List<IPermissionRegistration>();
        }

        public Task WriteHelpFileAsync()
        {
            var directory = m_Plugin.WorkingDirectory;
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            StringBuilder markdownBuilder = new StringBuilder();
            markdownBuilder.AppendLine($"# {m_Plugin.DisplayName}");
            markdownBuilder.AppendLine($"Id:  {m_Plugin.OpenModComponentId}  ");
            markdownBuilder.AppendLine($"Version:  {m_Plugin.Version}  ");
            if (!string.IsNullOrEmpty(m_Plugin.Author))
            {
                markdownBuilder.AppendLine($"Author: {m_Plugin.Author}  ");
            }
            if (!string.IsNullOrEmpty(m_Plugin.Website))
            {
                markdownBuilder.AppendLine($"Website: {m_Plugin.Website}  ");
            }

            var commands = m_CommandStore.Commands
                .Where(d => d.Component == m_Plugin)
                .ToList();

            var rootCommands = commands
                .Where(d => string.IsNullOrEmpty(d.ParentId))
                .ToList();

            if (rootCommands.Count > 0)
            {
                markdownBuilder.AppendLine();
                markdownBuilder.AppendLine("## Commands");
                foreach (var currentCommand in rootCommands)
                {
                    var args = new List<string> { currentCommand.Name };
                    AppendCommand(markdownBuilder, currentCommand, commands, args);
                }
            }

            var permissions = m_PermissionRegistry.GetPermissions(m_Plugin)
                .Where(d => !m_PrintedCommandPermissions.Any(e =>
                    e.Permission.Equals(d.Permission, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            if (permissions.Count > 0)
            {
                markdownBuilder.AppendLine();
                markdownBuilder.AppendLine("## Permissions");
                foreach (var permission in permissions)
                {
                    markdownBuilder.Append($"  - {permission.Owner.OpenModComponentId}.{permission.Permission}");
                    if (!string.IsNullOrEmpty(permission.Description))
                    {
                        markdownBuilder.Append($": {permission.Description}");
                    }

                    markdownBuilder.AppendLine("  ");
                }
            }

            var filePath = Path.Combine(directory, "help.md");
            File.WriteAllText(filePath, markdownBuilder.ToString());

            return Task.CompletedTask;
        }

        private void AppendCommand(
            StringBuilder markdownBuilder,
            ICommandRegistration command,
            List<ICommandRegistration> commands,
            List<string> args)
        {
            var ctx = m_CommandContextBuilder.CreateContext(null, args.ToArray(), string.Empty, commands);
            try
            {
                markdownBuilder.Append($"- {ctx.CommandPrefix}{ctx.CommandAlias}");
                if (!string.IsNullOrEmpty(command.Syntax))
                {
                    markdownBuilder.Append($" {command.Syntax}");
                }

                if (!string.IsNullOrEmpty(command.Description))
                {
                    markdownBuilder.Append($": {command.Description}");
                }

                markdownBuilder.AppendLine("  ");
                markdownBuilder.AppendLine($"  id: {command.Id}  ");
                var permissionRegistrations = new List<IPermissionRegistration>();

                var commandPermission = m_PermissionBuilder.GetPermission(command, commands);

                var commandPermissionRegistration = m_PermissionRegistry.FindPermission(m_Plugin, commandPermission);
                if (commandPermissionRegistration != null)
                {
                    permissionRegistrations.Add(commandPermissionRegistration);
                }

                if (command.PermissionRegistrations != null)
                {
                    var prefixedPermissions = new List<IPermissionRegistration>();

                    foreach (var permission in command.PermissionRegistrations)
                    {
                        prefixedPermissions.Add(new PermissionRegistration
                        {
                            Permission = commandPermission + "." + permission.Permission,
                            Description = permission.Description,
                            DefaultGrant = permission.DefaultGrant,
                            Owner = permission.Owner
                        });
                    }

                    permissionRegistrations.AddRange(prefixedPermissions);
                }

                if (permissionRegistrations.Count > 0)
                {
                    markdownBuilder.AppendLine($"  permissions: ");

                    foreach (var permissionRegistration in permissionRegistrations)
                    {
                        markdownBuilder.Append($"  - {permissionRegistration.Owner.OpenModComponentId}.{permissionRegistration.Permission}");
                        if (!string.IsNullOrEmpty(permissionRegistration.Description))
                        {
                            markdownBuilder.Append($": {permissionRegistration.Description}");
                        }

                        markdownBuilder.AppendLine("  ");
                    }

                    m_PrintedCommandPermissions.AddRange(permissionRegistrations);
                }

                var childCommands = commands
                    .Where(d => string.Equals(d.ParentId, command.Id, StringComparison.OrdinalIgnoreCase));
                foreach (var child in childCommands)
                {
                    args.Add(child.Name);
                    AppendCommand(markdownBuilder, child, commands, args);
                }
            }
            finally
            {
                ctx.DisposeAsync().GetAwaiter().GetResult();
            }
        }
    }
}