using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.Core.Users;

namespace OpenMod.Core.Console
{
    [OpenModInternal]
    public class ConsoleActor : ICommandActor
    {
        private readonly ILogger<ConsoleActor> m_Logger;
        public ConsoleActor(ILogger<ConsoleActor> logger, string consoleId)
        {
            m_Logger = logger;
            Id = consoleId;
        }

        public string Id { get; }
        public string Type { get; } = KnownActorTypes.Console;
        public Dictionary<string, object> Data { get; } = new();
        public string DisplayName { get; } = "Console";

        public Task PrintMessageAsync(string message)
        {
            return PrintMessageAsync(message, Color.White);
        }

        public Task PrintMessageAsync(string message, Color color)
        {
            // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
            m_Logger.LogInformation(message);
            return Task.CompletedTask;
        }

        public string FullActorName
        {
            get { return DisplayName; }
        }
    }
}