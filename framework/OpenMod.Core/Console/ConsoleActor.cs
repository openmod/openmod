using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMod.API.Commands;
using OpenMod.Core.Users;

namespace OpenMod.Core.Console
{
    public class ConsoleActor : ICommandActor
    {
        public bool IsGameConsole { get; }


        private readonly ILogger<ConsoleActor> m_Logger;
        public ConsoleActor(ILogger<ConsoleActor> logger, string consoleId, bool isGameConsole)
        {
            m_Logger = logger;

            IsGameConsole = isGameConsole;
            Id = consoleId;
        }

        public string Id { get; }
        public string Type { get; } = KnownActorTypes.Console;
        public Dictionary<string, object> Data { get; } = new Dictionary<string, object>();
        public string DisplayName { get; } = "Console";

        public Task PrintMessageAsync(string message)
        {
            return PrintMessageAsync(message, Color.White);
        }

        public Task PrintMessageAsync(string message, Color color)
        {
            m_Logger.LogInformation(message);
            return Task.CompletedTask;
        }
    }
}