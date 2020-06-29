using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Commands;
using OpenMod.API.Ioc;
using OpenMod.API.Localization;
using OpenMod.API.Prioritization;

namespace OpenMod.Core.Commands
{
    [UsedImplicitly]
    [ServiceImplementation(Lifetime = ServiceLifetime.Transient, Priority = Priority.Lowest)]
    public class CommandContextBuilder : ICommandContextBuilder
    {
        private readonly IOpenModStringLocalizer m_StringLocalizer;
        private readonly ILifetimeScope m_LifetimeScope;

        public CommandContextBuilder(
            IOpenModStringLocalizer stringLocalizer,
            ILifetimeScope lifetimeScope)
        {
            m_StringLocalizer = stringLocalizer;
            m_LifetimeScope = lifetimeScope;
        }

        public virtual CommandContext BuildContextTree(CommandContext currentContext, IEnumerable<ICommandRegistration> commandRegistrations)
        {
            if (currentContext.Parameters.Count == 0)
            {
                CheckCommandActor(ref currentContext);
                return currentContext;
            }

            var childCommandName = currentContext.Parameters.First();
            var children = CommandRegistrationHelper.GetChildren(currentContext.CommandRegistration, commandRegistrations);
            var childCommand = GetCommandRegistration(childCommandName, children);
            if (childCommand == null)
            {
                CheckCommandActor(ref currentContext);
                return currentContext;
            }

            var scope = childCommand.Component.LifetimeScope.BeginLifetimeScope();
            var childContext = new CommandContext(childCommand, scope, currentContext) { CommandRegistration = childCommand };
            currentContext.ChildContext = childContext;

            return BuildContextTree(childContext, commandRegistrations);
        }

        public ICommandContext CreateContext(ICommandActor actor, string[] args, string prefix, IEnumerable<ICommandRegistration> commandRegistrations)
        {
            var rootCommand = GetCommandRegistration(args[0], commandRegistrations.Where(d => d.ParentId == null));
            if (rootCommand == null)
            {
                var exceptionContext = new CommandContext(null, actor, args.First(), prefix, args.Skip(1).ToList(), m_LifetimeScope.BeginLifetimeScope());
                var localizer = m_LifetimeScope.Resolve<IOpenModStringLocalizer>();
                exceptionContext.Exception = new CommandNotFoundException(localizer["commands:errors:not_found", new { CommandName = args[0], Args = args }]);
                //await actor.PrintMessageAsync(Color.Red, exceptionContext.Exception.Message);
                return exceptionContext;
            }

            var scope = rootCommand.Component.LifetimeScope.BeginLifetimeScope($"Command context scope for \"{string.Join(" ", args)}\" by actor {actor.Type}/{actor.DisplayName} ({actor.Id})");
            var rootContext = new CommandContext(rootCommand, actor, args.First(), prefix, args.Skip(1).ToList(), scope);

            return BuildContextTree(rootContext, commandRegistrations);
        }

        private ICommandRegistration GetCommandRegistration(string name, IEnumerable<ICommandRegistration> commandRegistrations)
        {
            // todo: could be done in a single iteration
            // ReSharper disable PossibleMultipleEnumeration
            return commandRegistrations.FirstOrDefault(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                   ?? commandRegistrations.FirstOrDefault(d => d.Aliases != null && d.Aliases.Any(e => e.Equals(name, StringComparison.OrdinalIgnoreCase)));
            // ReSharper restore PossibleMultipleEnumeration
        }

        private void CheckCommandActor(ref CommandContext currentContext)
        {
            if (currentContext.CommandRegistration.SupportsActor(currentContext.Actor))
            {
                return;
            }
            
            var localizer = m_LifetimeScope.Resolve<IOpenModStringLocalizer>();
            currentContext.Exception = new UserFriendlyException(localizer["commands:errors:bad_actor",
                new {CommandName = currentContext.CommandAlias, ActorType = currentContext.Actor.Type}]);
        }
    }
}