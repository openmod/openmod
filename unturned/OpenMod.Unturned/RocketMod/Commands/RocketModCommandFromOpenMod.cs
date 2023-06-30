using System;
using System.Collections.Generic;
using System.Linq;
using OpenMod.API.Commands;
using Rocket.API;

namespace OpenMod.Unturned.RocketMod.Commands
{
    public class RocketModCommandFromOpenMod : IRocketCommand
    {
        public string Name { get; }
        public List<string> Aliases { get; }
        public AllowedCaller AllowedCaller { get; }
        public string Help { get; }
        public List<string> Permissions { get; }
        public string Syntax { get; }

        public RocketModCommandFromOpenMod(ICommandContext commandContext)
        {
            Name = commandContext.CommandAlias;

            Aliases = commandContext.CommandRegistration?.Aliases?.ToList() ?? new List<string>();
            Help = commandContext.CommandRegistration?.Description ?? string.Empty;
            Permissions = commandContext.CommandRegistration?.PermissionRegistrations?.Select(cm => cm.Permission).ToList() ?? new List<string>();
            Syntax = commandContext.CommandRegistration?.Syntax ?? string.Empty;

            AllowedCaller = AllowedCaller.Both;
        }

        public void Execute(IRocketPlayer player, string[] command)
        {
            throw new InvalidOperationException($"OpenmodCommand: {Name} can not be execute directly.");
        }
    }
}
