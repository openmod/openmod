using System.Collections.Generic;
using OpenMod.API;
using OpenMod.API.Commands;
using SDG.Unturned;

namespace OpenMod.Unturned.Commands
{
    public class UnturnedCommandSource : ICommandSource
    {
        public UnturnedCommandSource(IRuntime runtime)
        {
            Commands = new List<ICommandRegistration>();
            foreach (var cmd in Commander.commands)
            {
                Commands.Add(new UnturnedCommandRegistration(runtime, cmd));
            }
        }

        public ICollection<ICommandRegistration> Commands { get; }
    }
}