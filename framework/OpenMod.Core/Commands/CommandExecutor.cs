using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenMod.API.Commands;
using OpenMod.API.Ioc;
using OpenMod.API.Localization;
using OpenMod.API.Prioritization;

namespace OpenMod.Core.Commands
{
    [UsedImplicitly]
    [ServiceImplementation(Lifetime = ServiceLifetime.Transient, Priority = Priority.Lowest)]
    public class CommandExecutor : ICommandExecutor
    {
        private readonly ILifetimeScope m_LifetimeScope;
        private readonly IOptions<CommandExecutorOptions> m_Options;

        public CommandExecutor(
            ILifetimeScope lifetimeScope,
            IOptions<CommandExecutorOptions> options)
        {
            m_LifetimeScope = lifetimeScope;
            m_Options = options;
        }
        public async Task<ICommandContext> ExecuteAsync(ICommandActor actor, string[] args, string prefix)
        {
            if (args == null || args.Length == 0)
            {
                throw new Exception("Can not execute command with null or empty args");
            }


            await using var scope = m_LifetimeScope.BeginLifetimeScope();
            var currentCommandAccessor = scope.Resolve<ICurrentCommandContextAccessor>();
            var serviceProvider = scope.Resolve<IServiceProvider>();
            var commandSources = m_Options.Value.CreateCommandSources(serviceProvider);
            var commandsRegistrations = commandSources.SelectMany(d => d.Commands).ToList();
            var logger = scope.Resolve<ILogger<CommandExecutor>>();
            var rootContext = new CommandContext(actor, args)
            {
                CommandPrefix = prefix,
                CommandAlias = args[0],
                ServiceProvider = serviceProvider
            };
            var rootCommand = GetCommandRegistration(actor, args[0], commandsRegistrations.Where(d => d.ParentId == null));
            if (rootCommand == null)
            {
                var localizer = scope.Resolve<IOpenModStringLocalizer>();
                rootContext.Exception = new CommandNotFoundException(localizer["commands:not_found", new { CommandName = args[0], Args = args }]);
                await actor.PrintMessageAsync(Color.Red, rootContext.Exception.Message);
                return rootContext;
            }

            rootContext.CommandRegistration = rootCommand;
            var commandContext = BuildContext(rootContext, commandsRegistrations);

            try
            {
                currentCommandAccessor.Context = commandContext;

                var command = commandContext.CommandRegistration.Instantiate(scope);
                await command.ExecuteAsync();

                currentCommandAccessor.Context = null;
            }
            catch (UserFriendlyException ex)
            {
                await actor.PrintMessageAsync(Color.Red, ex.Message);
                commandContext.Exception = ex;
            }
            catch (Exception ex)
            {
                await actor.PrintMessageAsync(Color.Red, "An internal error occured during the command execution.");
                logger.LogError(ex, $"Exception occured on command \"{string.Join(" ", args)}\" by actor {actor.Type}/{actor.DisplayName} ({actor.Id})");
                commandContext.Exception = ex;
            }

            return commandContext;
        }

        public virtual CommandContext BuildContext(CommandContext currentContext, ICollection<ICommandRegistration> commandRegistrations)
        {
            if (currentContext.Parameters.Count == 0)
            {
                return currentContext;
            }

            string childCommandName = currentContext.Parameters.First();
            var childCommand = GetCommandRegistration(currentContext.Actor, childCommandName, GetChildren(currentContext.CommandRegistration, commandRegistrations));
            if (childCommand == null)
            {
                return currentContext;
            }

            var childContext = new CommandContext(currentContext) { CommandRegistration = childCommand };
            currentContext.ChildContext = childContext;

            return BuildContext(childContext, commandRegistrations);
        }

        protected virtual ICommandRegistration GetCommandRegistration(ICommandActor actor, string name, IEnumerable<ICommandRegistration> commandRegistrations)
        {
            return commandRegistrations.FirstOrDefault(d => d.SupportsActor(actor) && d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        protected virtual List<ICommandRegistration> GetChildren(ICommandRegistration registration, ICollection<ICommandRegistration> commandRegistrations)
        {
            var children = new List<ICommandRegistration>();
            foreach (var commandRegistration in commandRegistrations)
            {
                if (commandRegistration.ParentId != null && commandRegistration.ParentId.Equals(registration.Id, StringComparison.OrdinalIgnoreCase))
                {
                    children.Add(commandRegistration);
                }
            }

            return children;
        }
    }
}