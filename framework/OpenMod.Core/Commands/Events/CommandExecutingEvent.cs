using OpenMod.API.Commands;
using OpenMod.API.Eventing;
using OpenMod.Core.Eventing;

namespace OpenMod.Core.Commands.Events
{
    public sealed class CommandExecutingEvent : Event, ICancellableEvent
    {
        public ICommandActor Actor { get; }
        public ICommandContext CommandContext { get; }

        public CommandExecutingEvent(ICommandActor actor, ICommandContext commandContext)
        {
            Actor = actor;
            CommandContext = commandContext;
        }

        public bool IsCancelled { get; set; }
    }
}