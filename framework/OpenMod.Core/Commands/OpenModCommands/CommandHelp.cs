using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using OpenMod.API.Commands;
using OpenMod.API.Localization;

namespace OpenMod.Core.Commands.OpenModCommands
{
    [UsedImplicitly]
    [Command("help")]
    [CommandSummary("Get help on commands")]
    [CommandSyntax("[command | page number]")]
    public class CommandHelp : Command
    {
        private readonly IOptions<CommandExecutorOptions> m_CommandExecutorOptions;
        private readonly IServiceProvider m_ServiceProvider;
        private readonly ICommandContextBuilder m_CommandContextBuilder;

        public CommandHelp(
            ICurrentCommandContextAccessor contextAccessor,
            IOptions<CommandExecutorOptions> commandExecutorOptions,
            IServiceProvider serviceProvider,
            ICommandContextBuilder commandContextBuilder) : base(contextAccessor)
        {
            m_CommandExecutorOptions = commandExecutorOptions;
            m_ServiceProvider = serviceProvider;
            m_CommandContextBuilder = commandContextBuilder;
        }

        protected override async Task OnExecuteAsync()
        {
            var commandSources = m_CommandExecutorOptions.Value.CreateCommandSources(m_ServiceProvider);
            var commands = commandSources.SelectMany(d => d.Commands).ToList();

            const int amountPerPage = 10;

            int currentPage = 1;
            if (Context.Parameters.Length == 0 || Context.Parameters.TryGet(0, out currentPage))
            {
                if (currentPage < 1)
                {
                    throw new CommandWrongUsageException(Context);
                }

                var pageCommands = commands
                    .Where(d => d.ParentId == null)
                    .Skip(amountPerPage * (currentPage - 1))
                    .Take(amountPerPage);

                await PrintPageAsync(currentPage, (int)Math.Ceiling((double)commands.Count / amountPerPage), pageCommands);
            }
            else if (Context.Parameters.Length > 0)
            {
                var context = m_CommandContextBuilder.CreateContext(Context.Actor, Context.Parameters.ToArray(), Context.CommandPrefix, commands);
                await PrintCommandHelpAsync(context, commands);
            }
        }

        private async Task PrintCommandHelpAsync(ICommandContext context, List<ICommandRegistration> commands)
        {
            await PrintAsync($"Name: {context.CommandRegistration.Name}");
            var aliases = context.CommandRegistration.Aliases;
            if (aliases != null)
            {
                await PrintAsync($"Aliases: {string.Join(", ", aliases)}");
            }

            await PrintAsync($"Syntax: {context.CommandRegistration.Syntax}");

            if (!string.IsNullOrEmpty(context.CommandRegistration.Description))
            {
                await PrintAsync($"Description: {context.CommandRegistration.Description}");
            }

            await PrintAsync($"Permission: {context.CommandRegistration.Permission ?? "<none>"}");
            await PrintAsync("Command structure:");
            await PrintChildrenAsync(context.CommandRegistration, commands, "", true);
        }

        private async Task PrintChildrenAsync(ICommandRegistration registration, ICollection<ICommandRegistration> commands, string intent, bool isLast)
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
            if (!string.IsNullOrEmpty(registration.Summary))
            {
                sb.Append($": {registration.Summary}");
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

        private async Task PrintPageAsync(int pageNumber, int pageCount, IEnumerable<ICommandRegistration> page)
        {
            var commandRegistrations = page.ToList();
            if (commandRegistrations.Count == 0)
            {
                await PrintAsync("No commands found.", Color.Red);
                return;
            }

            await PrintAsync($"[{pageNumber}/{pageCount}] Commands", Color.CornflowerBlue);
            foreach (var command in commandRegistrations)
            {
                await PrintAsync(GetCommandUsage(command, Context.CommandPrefix));
            }
        }

        public string GetCommandUsage(ICommandRegistration command, string prefix)
        {
            return prefix
                   + command.Name.ToLower()
                   + (string.IsNullOrEmpty(command.Syntax) ? "" : " " + command.Syntax)
                   + (string.IsNullOrEmpty(command.Summary) ? "" : ": " + command.Summary);
        }
    }
}