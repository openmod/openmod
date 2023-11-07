using Autofac;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.API.Ioc;
using OpenMod.API.Localization;
using OpenMod.API.Prioritization;
using OpenMod.Core.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenMod.Core.Commands
{
    [OpenModInternal]
    [UsedImplicitly]
    [ServiceImplementation(Lifetime = ServiceLifetime.Transient, Priority = Priority.Lowest)]
    public class CommandContextBuilder : ICommandContextBuilder
    {
        private readonly ILifetimeScope m_LifetimeScope;

        public CommandContextBuilder(
            IOpenModStringLocalizer stringLocalizer,
            ILifetimeScope lifetimeScope)
        {
            m_LifetimeScope = lifetimeScope;
        }

        public virtual CommandContext BuildContextTree(CommandContext currentContext, IReadOnlyCollection<ICommandRegistration> commandRegistrations)
        {
            if (currentContext == null)
            {
                throw new ArgumentNullException(nameof(currentContext));
            }

            if (currentContext.Parameters.Count == 0)
            {
                return currentContext;
            }

            var childCommandName = currentContext.Parameters.First();
            if (currentContext.CommandRegistration == null)
            {
                return currentContext;
            }

            var children = CommandRegistrationHelper.GetChildren(currentContext.CommandRegistration, commandRegistrations);
            var childCommand = GetCommandRegistration(currentContext.Actor, childCommandName, children);
            if (childCommand == null)
            {
                return currentContext;
            }

            var scope = childCommand.Component.LifetimeScope.BeginLifetimeScopeEx();
            var childContext = new CommandContext(childCommand, scope, currentContext) { CommandRegistration = childCommand };
            currentContext.ChildContext = childContext;

            return BuildContextTree(childContext, commandRegistrations);
        }

        public ICommandContext CreateContext(ICommandActor actor, string[] args, string prefix, IReadOnlyCollection<ICommandRegistration> commandRegistrations)
        {
            if (actor == null)
            {
                throw new ArgumentNullException(nameof(actor));
            }

            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            var rootCommand = GetCommandRegistration(actor, args[0], commandRegistrations.Where(d => d.ParentId == null));
            if (rootCommand == null)
            {
                var exceptionContext = new CommandContext(null, actor, args.First(), prefix,  args.Skip(1).ToList(), m_LifetimeScope.BeginLifetimeScopeEx());
                var localizer = m_LifetimeScope.Resolve<IOpenModStringLocalizer>();
                exceptionContext.Exception = new CommandNotFoundException(localizer["commands:errors:not_found", new { CommandName = args[0], Args = args }]!);
                //await actor.PrintMessageAsync(Color.Red, exceptionContext.Exception.Message);
                return exceptionContext;
            }

            var scope = rootCommand.Component.LifetimeScope.BeginLifetimeScopeEx("AutofacWebRequest");
            var rootContext = new CommandContext(rootCommand, actor, args.First(), prefix, args.Skip(1).ToList(), scope);
            return BuildContextTree(rootContext, commandRegistrations);
        }

        private ICommandRegistration? GetCommandRegistration(ICommandActor? actor, string name, IEnumerable<ICommandRegistration> commandRegistrations)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var baseQuery = commandRegistrations;

            if (actor != null)
            {
                baseQuery = baseQuery.Where(d => d.SupportsActor(actor));
            }

            ICommandRegistration? aliasMatch = null;

            foreach (var registration in baseQuery)
            {
                if (registration.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return registration;

                if (registration.Aliases != null &&
                    registration.Aliases.Any(e => e.Equals(name, StringComparison.OrdinalIgnoreCase)))
                    aliasMatch = registration;
            }

            return aliasMatch;
        }
    }
}