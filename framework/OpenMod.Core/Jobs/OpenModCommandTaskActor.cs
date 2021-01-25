using System.Drawing;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMod.API.Commands;
using OpenMod.Core.Users;

namespace OpenMod.Core.Jobs
{
    public class OpenModCommandTaskActor : ICommandActor
    {
        private readonly ILogger m_Logger;

        public OpenModCommandTaskActor(ILogger logger)
        {
            m_Logger = logger;
        }

        public string Id { get; } = "JobCommandActor";

        public string Type { get; } = KnownActorTypes.Console;

        public string DisplayName { get; } = "OpenMod Task Executor";

        public Task PrintMessageAsync(string message)
        {
            m_Logger.LogInformation(message);
            return Task.CompletedTask;
        }

        public Task PrintMessageAsync(string message, Color color)
        {
            return PrintMessageAsync(message);
        }
    }
}