using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMod.API.Prioritization;

namespace OpenMod.Core.Commands.OpenModCommands
{
    [Command("openmod", Priority = Priority.Highest)]
    [CommandAlias("om")]
    [CommandDescription("Get information about OpenMod and manage it")]
    public class CommandOpenMod : Command
    {
        public CommandOpenMod(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override Task OnExecuteAsync()
        {
            return Task.FromException(new CommandWrongUsageException(Context));
        }
    }
}