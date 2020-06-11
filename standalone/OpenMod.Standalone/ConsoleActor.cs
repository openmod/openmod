using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMod.API.Commands;

namespace OpenMod.Standalone
{
    public class ConsoleActor : ICommandActor
    {
        private readonly ILogger<ConsoleActor> m_Logger;
        public ConsoleActor(ILogger<ConsoleActor> logger)
        {
            m_Logger = logger;
        }

        public string Id { get; } = "openmod-standalone-console";
        public string Type { get; } = "console";
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