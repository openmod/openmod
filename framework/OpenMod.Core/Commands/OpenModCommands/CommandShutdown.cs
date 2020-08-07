using System;
using System.Drawing;
using System.Threading.Tasks;
using JetBrains.Annotations;
using OpenMod.API;
using OpenMod.API.Prioritization;

namespace OpenMod.Core.Commands.OpenModCommands
{
    [UsedImplicitly]
    [Command("shutdown", Priority = Priority.Low)]
    [CommandAlias("stop")]
    [CommandDescription("Shuts the application down")]
    public class CommandShutdown : Command
    {
        private readonly IOpenModHost m_Host;

        public CommandShutdown(IOpenModHost host, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            m_Host = host;
        }

        protected override async Task OnExecuteAsync()
        {
            await PrintAsync("Shutting down...", Color.Red);
            await m_Host.ShutdownAsync();
        }
    }
}