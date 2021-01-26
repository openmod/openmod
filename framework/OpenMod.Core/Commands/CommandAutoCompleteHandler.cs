using System;
using System.Collections.Generic;
using System.Linq;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.Core.Console;
using OpenMod.Core.Helpers;

namespace OpenMod.Core.Commands
{
    [OpenModInternal]
    public class CommandAutoCompleteHandler : IAutoCompleteHandler
    {
        private readonly IConsoleActorAccessor m_ConsoleActorAccessor;
        private readonly ICommandStore m_CommandStore;
        private readonly ICommandContextBuilder m_CommandContextBuilder;

        public CommandAutoCompleteHandler(
            IConsoleActorAccessor consoleActorAccessor,
            ICommandStore commandStore, 
            ICommandContextBuilder commandContextBuilder)
        {
            m_ConsoleActorAccessor = consoleActorAccessor;
            m_CommandStore = commandStore;
            m_CommandContextBuilder = commandContextBuilder;
        }

        public string[] GetSuggestions(string text, int index)
        {
            var args = text.Substring(0, index).Split(' ').ToArray();
            var commandStart = index >= text.Length ? string.Empty : text.Substring(index, text.Length - index);

            var commands = AsyncHelper.RunSync(() => m_CommandStore.GetCommandsAsync());
            var context = m_CommandContextBuilder.CreateContext(m_ConsoleActorAccessor.Actor, args, string.Empty, commands);

            IEnumerable<ICommandRegistration> matchingCommands;
            if (context.CommandRegistration == null)
            {
                matchingCommands = commands.Where(d =>
                    d.ParentId == null 
                    && d.Name.StartsWith(commandStart, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                matchingCommands = commands.Where(d =>
                    !string.IsNullOrEmpty(d.ParentId)
                    && (d.ParentId?.Equals(context.CommandRegistration.Id, StringComparison.OrdinalIgnoreCase) ?? false)
                    && d.Name.StartsWith(commandStart, StringComparison.OrdinalIgnoreCase));
            }

            AsyncHelper.RunSync(async () => await context.DisposeAsync());
            return matchingCommands.Select(d => d.Name).ToArray();
        }

        public char[] Separators { get; set; } = {' '};
    }
}