using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.API.Localization;
using OpenMod.API.Permissions;

// ReSharper disable PossibleMultipleEnumeration

namespace OpenMod.Core.Commands.OpenModCommands
{
    [UsedImplicitly]
    [Command("help")]
    [CommandDescription("Get help on commands")]
    [CommandSyntax("[command | page number]")]
    public class CommandHelp : Command
    {
        private readonly IRuntime m_Runtime;
        private readonly ICommandStore m_CommandStore;
        private readonly IPermissionRegistry m_PermissionRegistry;
        private readonly ICommandPermissionBuilder m_CommandPermissionBuilder;
        private readonly ICommandContextBuilder m_CommandContextBuilder;
        private readonly IOpenModStringLocalizer m_StringLocalizer;
        private readonly IPermissionChecker m_PermissionChecker;

        public CommandHelp(
            IRuntime runtime,
            ICommandStore commandStore,
            IServiceProvider serviceProvider,
            IPermissionRegistry permissionRegistry,
            ICommandPermissionBuilder commandPermissionBuilder,
            ICommandContextBuilder commandContextBuilder,
            IOpenModStringLocalizer stringLocalizer) : base(serviceProvider)
        {
            m_Runtime = runtime;

            // get global permission checker instead of scoped
            m_PermissionChecker = m_Runtime.Host.Services.GetRequiredService<IPermissionChecker>();

            m_CommandStore = commandStore;
            m_PermissionRegistry = permissionRegistry;
            m_CommandPermissionBuilder = commandPermissionBuilder;
            m_CommandContextBuilder = commandContextBuilder;
            m_StringLocalizer = stringLocalizer;
        }

        protected override async Task OnExecuteAsync()
        {
            var commands = await m_CommandStore.GetCommandsAsync();
            var totalCount = commands.Count;

            const int itemsPerPage = 10;

            int currentPage = 1;
            if (Context.Parameters.Length == 0 || Context.Parameters.TryGet(0, out currentPage))
            {
                if (currentPage < 1)
                {
                    throw new CommandWrongUsageException(Context);
                }

                var pageCommands = commands
                    .Where(d => d.ParentId == null)
                    .Skip(itemsPerPage * (currentPage - 1))
                    .Take(itemsPerPage)
                    .ToList();

                await PrintPageAsync(currentPage, (int)Math.Ceiling((double)totalCount / itemsPerPage), pageCommands);
            }
            else if (Context.Parameters.Length > 0)
            {
                var context = m_CommandContextBuilder.CreateContext(Context.Actor, Context.Parameters.ToArray(), Context.CommandPrefix, commands);
                var permission = GetPermission(context.CommandRegistration, commands);

                if (context.CommandRegistration == null)
                {
                    await Context.Actor.PrintMessageAsync(m_StringLocalizer["commands:errors:not_found", new { CommandName = context.GetCommandLine(false) }], Color.Red);
                    return;
                }

                if (!string.IsNullOrEmpty(permission) && await m_PermissionChecker.CheckPermissionAsync(Context.Actor, permission) != PermissionGrantResult.Grant)
                {
                    throw new NotEnoughPermissionException(m_StringLocalizer, permission);
                }

                await PrintCommandHelpAsync(context, permission, commands);
                await context.DisposeAsync();
            }
        }

        protected virtual string GetPermission(ICommandRegistration commandRegistration, IReadOnlyCollection<ICommandRegistration> commands)
        {
            var permission = commandRegistration == null ? null : m_CommandPermissionBuilder.GetPermission(commandRegistration, commands).Split(':')[1];
            if (permission == null)
            {
                return null;
            }

            var registeredPermission = m_PermissionRegistry.FindPermission(commandRegistration.Component, permission);
            if (registeredPermission == null)
            {
                throw new Exception($"Unregistered permission \"{permission}\" in component: {commandRegistration.Component.OpenModComponentId}");
            }

            return $"{registeredPermission.Owner.OpenModComponentId}:{registeredPermission.Permission}";
        }

        private async Task PrintCommandHelpAsync(ICommandContext context, string permission, IEnumerable<ICommandRegistration> commands)
        {
            string usage = $"Usage: {context.CommandRegistration.Name}";
            if (!string.IsNullOrEmpty(context.CommandRegistration.Syntax))
            {
                usage += $" {context.CommandRegistration.Syntax}";
            }

            await PrintAsync(usage);
            var aliases = context.CommandRegistration.Aliases;
            if (aliases != null && aliases.Count > 0)
            {
                await PrintAsync($"Aliases: {string.Join(", ", aliases)}");
            }

            if (!string.IsNullOrEmpty(context.CommandRegistration.Description))
            {
                await PrintAsync($"Description: {context.CommandRegistration.Description}");
            }

            await PrintAsync($"Permission: {permission ?? "<none>"}");
            await PrintAsync("Command structure:");
            await PrintChildrenAsync(context.CommandRegistration, commands, string.Empty, true);
        }

        private async Task PrintChildrenAsync(ICommandRegistration registration, IEnumerable<ICommandRegistration> commands, string intent, bool isLast)
        {
            var children = commands
                .Where(d => d.ParentId != null && d.ParentId.Equals(registration.Id, StringComparison.OrdinalIgnoreCase))
                .ToList();

            StringBuilder sb = new StringBuilder();
            sb.Append(intent);

            if (isLast)
            {
                sb.Append("┗ ");
                intent += " ";
            }
            else
            {
                sb.Append("┣ ");
                intent += "┃ ";
            }

            sb.Append(registration.Name);
            if (!string.IsNullOrEmpty(registration.Description))
            {
                sb.Append($": {registration.Description}");
            }

            await PrintAsync(sb.ToString());

            if (children.Count > 0)
            {
                int i = 1;
                foreach (var child in children)
                {
                    await PrintChildrenAsync(child, commands, intent, i == children.Count);
                    i++;
                }
            }
        }

        private async Task PrintPageAsync(int pageNumber, int pageCount, ICollection<ICommandRegistration> page)
        {
            await PrintAsync($"[{pageNumber}/{pageCount}] Commands", Color.CornflowerBlue);

            if (page.Count == 0)
            {
                await PrintAsync("No commands found.", Color.Red);
                return;
            }

            foreach (var command in page)
            {
                await PrintAsync(GetCommandUsage(command, Context.CommandPrefix));
            }
        }

        public string GetCommandUsage(ICommandRegistration command, string prefix)
        {
            return prefix
                   + command.Name.ToLower()
                   + (string.IsNullOrEmpty(command.Syntax) ? string.Empty : " " + command.Syntax)
                   + (string.IsNullOrEmpty(command.Description) ? string.Empty : ": " + command.Description);
        }
    }
}