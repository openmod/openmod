using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        private readonly ICommandPermissionBuilder m_PermissionBuilder;
        private readonly IPermissionRegistry m_PermissionRegistry;

        public PluginHelpWriter(
            ICommandPermissionBuilder permissionBuilder,
            IPermissionRegistry permissionRegistry,
            IOpenModPlugin plugin,
            ICommandStore commandStore)
        {
            m_PermissionBuilder = permissionBuilder;
            m_PermissionRegistry = permissionRegistry;
            m_Plugin = plugin;
            m_CommandStore = commandStore;
        }

        public async Task WriteHelpFileAsync()
        {
            var markdownBuilder = new StringBuilder();

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
            if (!string.IsNullOrEmpty(m_Plugin.Description))
            {
                markdownBuilder.Append("Description: ").AppendLine(m_Plugin.Description);
            }

            var commands = (await m_CommandStore.GetCommandsAsync())
                .Where(d => d.Component == m_Plugin)
                .ToList();

            var rootCommands = commands
                .Where(d => string.IsNullOrEmpty(d.ParentId))
                .ToList();

            if (rootCommands.Count > 0)
            {
                markdownBuilder.AppendLine();
                markdownBuilder.AppendLine("## Commands");
                foreach (var command in rootCommands)
                {
                    AppendCommand(markdownBuilder, command, commands, string.Empty);
                }
            }

            var permissions = m_PermissionRegistry.GetPermissions(m_Plugin);
            if (permissions.Count > 0)
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
            string prefix)
        {
            markdownBuilder.Append("- ").Append(prefix).Append(command.Name);
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

            if (command.PermissionRegistrations?.Count > 0)
            {
                var commandPermission = m_PermissionBuilder.GetPermission(command, commands);

                markdownBuilder.AppendLine("  permissions:");
                foreach (var permissionRegistration in command.PermissionRegistrations)
                {
                    markdownBuilder.Append("  - ").Append(commandPermission).Append('.').Append(permissionRegistration.Permission);
                    if (!string.IsNullOrEmpty(permissionRegistration.Description))
                    {
                        markdownBuilder.Append(": ").Append(permissionRegistration.Description);
                    }

                    markdownBuilder.AppendLine();
                }
            }

            prefix += $"{command.Name} ";

            var childCommands = commands
                .Where(d => string.Equals(d.ParentId, command.Id, StringComparison.OrdinalIgnoreCase));
            foreach (var child in childCommands)
            {
                AppendCommand(markdownBuilder, child, commands, prefix);
            }
        }
    }
}