using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenMod.API.Commands;

namespace OpenMod.Core.Commands
{
    public class CommandContext : ICommandContext
    {
        public CommandContext(ICommandActor actor, ICollection<string> args)
        {
            Actor = actor;
            RootContext = this;
            Parameters = new CommandParameters(this, args);
            Data = new Dictionary<string, object>();
        }
        public CommandContext(CommandContext parent)
        {
            ParentContext = parent;
            RootContext = parent.RootContext;
            ServiceProvider = parent.ServiceProvider;
            Actor = parent.Actor;
            Parameters = new CommandParameters(this, parent.Parameters.Skip(1).ToList());
            CommandAlias = parent.Parameters[1];
            CommandPrefix = $"{parent.CommandPrefix} {CommandAlias} ";
            Data = parent.Data;
        }

        public IServiceProvider ServiceProvider { get; set; }
        public ICommandContext ParentContext { get; set; }
        public ICommandContext ChildContext { get; set; }
        public ICommandContext RootContext { get; set; }
        public ICommandRegistration CommandRegistration { get; set; }
        public string CommandPrefix { get; set; }
        public string CommandAlias { get; set; }
        public ICommandActor Actor { get; set; }
        public ICommandParameters Parameters { get; }
        public Exception Exception { get; set; }

        public Dictionary<string, object> Data { get; }

        public Task PrintCommandUsageAsync()
        {
            throw new NotImplementedException();
        }
    }
}