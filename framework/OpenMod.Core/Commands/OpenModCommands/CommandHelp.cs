using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.API.Localization;
using OpenMod.API.Permissions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable PossibleMultipleEnumeration

namespace OpenMod.Core.Commands.OpenModCommands
{
    [UsedImplicitly]
    [Command("help")]
    [CommandDescription("Get help on commands")]
    [CommandSyntax("[command | page number]")]
    public class CommandHelp : Command
    {
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
            // get global permission checker instead of scoped
            m_PermissionChecker = runtime.Host!.Services.GetRequiredService<IPermissionChecker>();

            m_CommandStore = commandStore;
            m_PermissionRegistry = permissionRegistry;
            m_CommandPermissionBuilder = commandPermissionBuilder;
            m_CommandContextBuilder = commandContextBuilder;
            m_StringLocalizer = stringLocalizer;
        }

        protected override async Task OnExecuteAsync()
        {
            var commands = await m_CommandStore.GetCommandsAsync();

            const int itemsPerPage = 10;

            var currentPage = 1;
            if (Context.Parameters.Length == 0 || Context.Parameters.TryGet(0, out currentPage))
            {
                if (currentPage < 1)
                {
                    throw new CommandWrongUsageException(Context);
                }

                var parents = commands.Where(x => x.ParentId is null).ToList();

                var pageCommands = parents
                    .Skip(itemsPerPage * (currentPage - 1))
                    .Take(itemsPerPage);

                await PrintPageAsync(currentPage, (int)Math.Ceiling((double)parents.Count / itemsPerPage), pageCommands);
            }
            else if (Context.Parameters.Length > 0)
            {
                await using var context = m_CommandContextBuilder.CreateContext(Context.Actor, Context.Parameters.ToArray(), string.Empty, commands);
                if (context.CommandRegistration == null)
                {
                    await PrintAsync(m_StringLocalizer["commands:errors:not_found",
                        new { CommandName = context.GetCommandLine(includeArguments: false) }]!, Color.Red);
                    return;
                }

                var permission = GetPermission(context.CommandRegistration, commands);
                if (!string.IsNullOrEmpty(permission)
                    && await m_PermissionChecker.CheckPermissionAsync(Context.Actor, permission!) != PermissionGrantResult.Grant)
                {
                    throw new NotEnoughPermissionException(m_StringLocalizer, permission!);
                }

                await PrintCommandHelpAsync(context, permission, commands);
            }
        }

        protected virtual string? GetPermission(ICommandRegistration commandRegistration, IReadOnlyCollection<ICommandRegistration> commands)
        {
            var permission = m_CommandPermissionBuilder.GetPermission(commandRegistration, commands)?.Split(':')?[1];
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

        private async Task PrintCommandHelpAsync(ICommandContext context, string? permission, IReadOnlyCollection<ICommandRegistration> commands)
        {
            var usage = $"Usage: {context.CommandRegistration!.Name}";
            if (!string.IsNullOrEmpty(context.CommandRegistration.Syntax))
            {
                usage += $" {context.CommandRegistration.Syntax}";
            }

            await PrintAsync(usage);
            var aliases = context.CommandRegistration.Aliases;
            if (aliases?.Count > 0)
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

        private async Task PrintChildrenAsync(ICommandRegistration registration,
            IReadOnlyCollection<ICommandRegistration> commands, string intent, bool isLast)
        {
            var children = commands
                .Where(d => d.ParentId?.Equals(registration.Id, StringComparison.OrdinalIgnoreCase) == true)
                .ToList();

            var sb = new StringBuilder();
            sb.Append(intent);

            if (isLast)
            {
                sb.Append("┗ ");
                intent += "  ";
            }
            else
            {
                sb.Append("┣ ");
                intent += "┃ ";
            }

            sb.Append(registration.Name);
            if (!string.IsNullOrEmpty(registration.Description))
            {
                sb.Append(": ").Append(registration.Description);
            }

            await PrintAsync(sb.ToString());

            if (children.Count > 0)
            {
                var i = 1;
                foreach (var child in children)
                {
                    await PrintChildrenAsync(child, commands, intent, i == children.Count);
                    i++;
                }
            }
        }

        private async Task PrintPageAsync(int pageNumber, int pageCount, IEnumerable<ICommandRegistration> page)
        {
            await PrintAsync($"[{pageNumber}/{pageCount}] Commands", Color.CornflowerBlue);

            if (!page.Any())
            {
                await PrintAsync("No commands found.", Color.Red);
                return;
            }

            foreach (var command in page)
            {
                await PrintAsync(GetCommandUsage(command));
            }
        }

        public string GetCommandUsage(ICommandRegistration command)
        {
            return command.Name.ToLower()
                   + (string.IsNullOrEmpty(command.Syntax) ? string.Empty : " " + command.Syntax)
                   + (string.IsNullOrEmpty(command.Description) ? string.Empty : ": " + command.Description);
        }
    }
}