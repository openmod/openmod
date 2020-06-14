using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.API.Console;
using OpenMod.API.Prioritization;
using OpenMod.Unturned.API;
using SDG.Unturned;

namespace OpenMod.Unturned.Commands
{
    public class UnturnedCommandRegistration : ICommandRegistration
    {
        public UnturnedCommandRegistration(IOpenModComponent component, Command cmd)
        {
            Component = component;
            Cmd = cmd;
            Name = cmd.command;
            Syntax = cmd.help;
            Description = cmd.info;
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
        public Priority Priority { get;} = Priority.Low;
        public string ParentId { get; } = null;

        public bool SupportsActor(ICommandActor actor)
        {
            return actor is IUnturnedPlayerActor || actor is IConsoleActor;
        }

        public ICommand Instantiate(IServiceProvider serviceProvider)
        {
            return ActivatorUtilities.CreateInstance<UnturnedBuiltinCommand>(serviceProvider, this);
        }
    }
}