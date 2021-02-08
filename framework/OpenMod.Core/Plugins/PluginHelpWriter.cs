using OpenMod.API.Commands;
using OpenMod.API.Permissions;
using OpenMod.API.Plugins;
using OpenMod.Core.Permissions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenMod.Core.Users;

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

        public async Task WriteHelpFileAsync()
        {
            StringBuilder markdownBuilder = new StringBuilder();

            markdownBuilder.Append("# ").AppendLine(m_Plugin.DisplayName);
            markdownBuilder.Append("Id: ").AppendLine(m_Plugin.OpenModComponentId);
            markdownBuilder.Append("Version: ").AppendLine(m_Plugin.Version.ToString());
            if (!string.IsNullOrEmpty(m_Plugin.Author))
            {
                markdownBuilder.Append("Author: ").AppendLine(m_Plugin.Author);
            }
            if (!string.IsNullOrEmpty(m_Plugin.Website))
            {
                markdownBuilder.Append("Website: ").AppendLine(m_Plugin.Website);
            }

            var commands = (await m_CommandStore.GetCommandsAsync())
                .Where(d => d.Component == m_Plugin)
                .ToList();

            var rootCommands = commands
                .Where(d => string.IsNullOrEmpty(d.ParentId))
                .ToList();

            if (rootCommands.Any())
            {
                markdownBuilder.AppendLine();
                markdownBuilder.AppendLine("## Commands");
                foreach (var currentCommand in rootCommands)
                {
                    var actor = new PseudoActor();
                    var ctx = m_CommandContextBuilder.CreateContext(actor, new[] { currentCommand.Name }, string.Empty, commands);
                    AppendCommand(markdownBuilder, currentCommand, commands, ctx);
                }
            }

            var permissions = m_PermissionRegistry.GetPermissions(m_Plugin)
                .Where(d => !m_PrintedCommandPermissions.Any(e =>
                    e.Permission.Equals(d.Permission, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            if (permissions.Any())
            {
                markdownBuilder.AppendLine();
                markdownBuilder.AppendLine("## Permissions");
                foreach (var permission in permissions)
                {
                    markdownBuilder.Append("- ").Append(permission.Owner.OpenModComponentId).Append(':').Append(permission.Permission);
                    if (!string.IsNullOrEmpty(permission.Description))
                    {
                        markdownBuilder.Append(": ").Append(permission.Description);
                    }

                    markdownBuilder.AppendLine();
                }
            }

            var directory = m_Plugin.WorkingDirectory;
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var filePath = Path.Combine(directory, "help.md");
            File.WriteAllText(filePath, markdownBuilder.ToString());
        }

        private void AppendCommand(
            StringBuilder markdownBuilder,
            ICommandRegistration command,
            List<ICommandRegistration> commands,
            ICommandContext commandContext)
        {
            markdownBuilder.Append("- ").Append(commandContext.CommandPrefix).Append(commandContext.CommandAlias);
            if (!string.IsNullOrEmpty(command.Syntax))
            {
                markdownBuilder.Append(' ').Append(command.Syntax);
            }

            if (!string.IsNullOrEmpty(command.Description))
            {
                markdownBuilder.Append(": ").Append(command.Description);
            }

            markdownBuilder.AppendLine();
            markdownBuilder.Append("  id: ").AppendLine(command.Id);
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
                markdownBuilder.AppendLine("  permissions:");

                foreach (var permissionRegistration in permissionRegistrations)
                {
                    markdownBuilder.Append("  - ").Append(permissionRegistration.Permission);
                    if (!string.IsNullOrEmpty(permissionRegistration.Description))
                    {
                        markdownBuilder.Append(": ").Append(permissionRegistration.Description);
                    }

                    markdownBuilder.AppendLine();
                }

                m_PrintedCommandPermissions.AddRange(permissionRegistrations);
            }

            var childCommands = commands
                .Where(d => string.Equals(d.ParentId, command.Id, StringComparison.OrdinalIgnoreCase));
            foreach (var child in childCommands)
            {
                var actor = new PseudoActor();
                var ctx2 = m_CommandContextBuilder.CreateContext(actor, new[] { command.Name, child.Name }, string.Empty, commands);
                AppendCommand(markdownBuilder, child, commands, ctx2);
            }
            commandContext.DisposeAsync().GetAwaiter().GetResult();
        }

        public class PseudoActor : ICommandActor
        {
            public string Id { get; } = "PseudoActor";

            public string Type { get; } = KnownActorTypes.Console;

            public string DisplayName { get; } = "PseudoActor";

            public string FullActorName
            {
                get
                {
                    return DisplayName;
                }
            }

            public Task PrintMessageAsync(string message)
            {
                throw new NotSupportedException();
            }

            public Task PrintMessageAsync(string message, Color color)
            {
                throw new NotSupportedException();
            }
        }
    }
}