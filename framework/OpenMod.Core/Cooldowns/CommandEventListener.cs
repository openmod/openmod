using System;
using System.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.API.Eventing;
using OpenMod.API.Localization;
using OpenMod.Core.Commands.Events;
using OpenMod.Core.Eventing;

namespace OpenMod.Core.Cooldowns
{
    public class CommandEventListener : IEventListener<CommandExecutingEvent>, IEventListener<CommandExecutedEvent>
    {
        private readonly ICommandCooldownStore m_CommandCooldownStore;
        private readonly IOpenModStringLocalizer m_StringLocalizer;

        public CommandEventListener(
            ICommandCooldownStore commandCooldownStore,
            IOpenModStringLocalizer stringLocalizer)
        {
            m_CommandCooldownStore = commandCooldownStore;
            m_StringLocalizer = stringLocalizer;
        }

        [EventListener(Priority = EventListenerPriority.High)]
        public async Task HandleEventAsync(object? sender, CommandExecutingEvent @event)
        {
            var id = @event.CommandContext.CommandRegistration?.Id;
            if (id == null)
            {
                return;
            }

            var cooldownSpan = await m_CommandCooldownStore.GetCooldownSpanAsync(@event.Actor, id);

            if (cooldownSpan.HasValue)
            {
                var lastExecuted = await m_CommandCooldownStore.GetLastExecutedAsync(@event.Actor, id);

                if (lastExecuted.HasValue)
                {
                    var spanSinceLast = DateTime.Now - lastExecuted.Value;
                    if (spanSinceLast < cooldownSpan)
                    {
                        @event.CommandContext.Exception = new UserFriendlyException(m_StringLocalizer["commands:errors:cooldown", new { TimeLeft = cooldownSpan - spanSinceLast }]!);
                    }
                }
            }
        }

        public async Task HandleEventAsync(object? sender, CommandExecutedEvent @event)
        {
            if (@event.CommandContext.CommandRegistration == null)
            {
                // Unknown command
                return;
            }

            if (@event.CommandContext.Exception == null || @event.ExceptionHandled)
            {
                // Command was successfully executed
                await m_CommandCooldownStore.RecordExecutionAsync(@event.Actor, @event.CommandContext.CommandRegistration.Id, DateTime.Now);
            }
        }
    }
}