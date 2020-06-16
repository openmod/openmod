using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using OpenMod.API.Commands;

namespace OpenMod.Core.Commands
{
    public class CommandContext : ICommandContext
    {
        public CommandContext(ICommandRegistration command, ICommandActor actor, string alias, string prefix, ICollection<string> args, ILifetimeScope scope)
        {
            CommandPrefix = prefix ?? string.Empty;
            CommandAlias = alias;
            LifetimeScope = scope;
            CommandRegistration = command;
            ServiceProvider = scope?.Resolve<IServiceProvider>();
            Actor = actor;
            RootContext = this;
            Parameters = new CommandParameters(this, args);
            Data = new Dictionary<string, object>();
        }

        public CommandContext(ICommandRegistration command, ILifetimeScope scope, CommandContext parent)
        {
            LifetimeScope = scope;
            CommandRegistration = command;
            ParentContext = parent;
            RootContext = parent.RootContext;
            ServiceProvider = scope.Resolve<IServiceProvider>();
            Actor = parent.Actor;
            Parameters = new CommandParameters(this, parent.Parameters.Skip(1).ToList());
            CommandAlias = parent.Parameters[0];
            CommandPrefix = $"{parent.CommandPrefix}{parent.CommandAlias} ";
            Data = parent.Data;
        }

        public IServiceProvider ServiceProvider { get; }
        public ICommandContext ParentContext { get; set; }
        public ICommandContext ChildContext { get; set; }
        public ICommandContext RootContext { get; set; }
        public ICommandRegistration CommandRegistration { get; set; }
        public string CommandPrefix { get; }
        public string CommandAlias { get; }
        public ICommandActor Actor { get; set; }
        public ICommandParameters Parameters { get; }
        public Exception Exception { get; set; }

        public Dictionary<string, object> Data { get; }
        public ILifetimeScope LifetimeScope { get; }

        public async ValueTask DisposeAsync()
        {
            if (LifetimeScope != null)
            {
                await LifetimeScope.DisposeAsync();
            }

            var current = this;
            while ((current = (CommandContext) current.ParentContext) != null)
            {
                await current.DisposeAsync();
            }
        }
    }
}