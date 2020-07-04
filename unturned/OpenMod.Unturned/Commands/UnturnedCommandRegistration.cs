using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.API.Prioritization;
using OpenMod.Core.Users;
using SDG.Unturned;

namespace OpenMod.Unturned.Commands
{
    public class UnturnedCommandRegistration : ICommandRegistration
    {
        public UnturnedCommandRegistration(IOpenModComponent component, Command cmd)
        {
            Component = component;
            Cmd = cmd;
            Name = cmd.command.ToLower(CultureInfo.InvariantCulture);
            Syntax = string.Join(" ", cmd.info.Split(' ').Skip(1)); /* skip first word in info, which is the command */
            Description = cmd.help;
            Aliases = new List<string>();
            Id = cmd.GetType().FullName;
        }

        public IOpenModComponent Component { get; }
        public Command Cmd { get; }
        public string Name { get; }
        public IReadOnlyCollection<string> Aliases { get; }
        public string Description { get; }
        public string Syntax { get; }
        public string Id { get; }
        public Priority Priority { get;} = Priority.Lowest;
        public string ParentId { get; } = null;

        public bool SupportsActor(ICommandActor actor)
        {
            return actor.Type == KnownActorTypes.Player || actor.Type == KnownActorTypes.Console;
        }

        public ICommand Instantiate(IServiceProvider serviceProvider)
        {
            return ActivatorUtilities.CreateInstance<UnturnedBuiltinCommand>(serviceProvider, this);
        }
    }
}