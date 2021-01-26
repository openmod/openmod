using OpenMod.API.Commands;
using OpenMod.Core.Eventing;

namespace OpenMod.Core.Commands.Events
{
    public sealed class CommandExecutedEvent : Event
    {
        public ICommandActor Actor { get; }
        public ICommandContext CommandContext { get; }
        public bool ExceptionHandled { get; set; }
        
        public CommandExecutedEvent(ICommandActor actor, ICommandContext commandContext)
        {
            Actor = actor;
            CommandContext = commandContext;
        }
    }
}