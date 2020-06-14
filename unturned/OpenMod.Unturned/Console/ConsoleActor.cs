using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMod.API.Commands;
using OpenMod.Core.Users;

namespace OpenMod.Unturned.Console
{
    public class ConsoleActor : ICommandActor
    {
        private readonly ILogger<ConsoleActor> m_Logger;
        public ConsoleActor(ILogger<ConsoleActor> logger)
        {
            m_Logger = logger;
        }

        public string Id { get; } = "openmod-unturned-console";
        public string Type { get; } = KnownUserTypes.Console;
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