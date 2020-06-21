using System;
using System.Drawing;
using System.Threading.Tasks;
using OpenMod.API;
using OpenMod.API.Prioritization;

namespace OpenMod.Core.Commands.OpenModCommands
{
    [Command("openmod", Priority = Priority.Highest)]
    [CommandAlias("om")]
    [CommandDescription("Get information about OpenMod and manage it")]
    [CommandSyntax("[<install/uninstall/update/reload>")]
    public class CommandOpenMod : Command
    {
        private readonly IRuntime m_Runtime;
        private readonly IOpenModHost m_Host;

        public CommandOpenMod(IRuntime runtime, IOpenModHost host, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            m_Runtime = runtime;
            m_Host = host;
        }

        protected override async Task OnExecuteAsync()
        {
            await Context.Actor.PrintMessageAsync($"OpenMod v{m_Runtime.Version} (c) Enes Sadık Özbek", Color.Green);
            await Context.Actor.PrintMessageAsync("OpenMod is free and open source, licensed under the EUPL-1.2.", Color.Green);
            await Context.Actor.PrintMessageAsync("Downloads and source code available at https://github.com/openmod/OpenMod.", Color.Green);
            await Context.Actor.PrintMessageAsync($"Host: {m_Host.Name} v{m_Host.Version}", Color.Green);
        }
    }
}