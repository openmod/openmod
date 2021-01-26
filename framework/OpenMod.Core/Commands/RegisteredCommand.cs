using System;
using System.Collections.Generic;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.API.Permissions;
using OpenMod.API.Prioritization;

namespace OpenMod.Core.Commands
{
    public class RegisteredCommand : ICommandRegistration
    {
        public ICommandRegistration BaseCommandRegistration { get; }
        public RegisteredCommandData CommandData { get; }

        public RegisteredCommand(ICommandRegistration baseCommandRegistration, RegisteredCommandData commandData)
        {
            BaseCommandRegistration = baseCommandRegistration;
            CommandData = commandData;
        }

        public IOpenModComponent Component => BaseCommandRegistration.Component;

        public string Id => BaseCommandRegistration.Id;

        public string Name => CommandData?.Name ?? BaseCommandRegistration.Name;

        public IReadOnlyCollection<string> Aliases => CommandData.Aliases ?? BaseCommandRegistration.Aliases!;

        public IReadOnlyCollection<IPermissionRegistration> PermissionRegistrations => BaseCommandRegistration.PermissionRegistrations!;

        public string? Description => BaseCommandRegistration.Description;

        public string? Syntax => BaseCommandRegistration.Syntax;

        public Priority Priority => CommandData.Priority ?? BaseCommandRegistration.Priority;

        public string? ParentId => CommandData.ParentId ?? BaseCommandRegistration.ParentId;

        public bool SupportsActor(ICommandActor actor)
        {
            return BaseCommandRegistration.SupportsActor(actor);
        }

        public ICommand Instantiate(IServiceProvider serviceProvider)
        {
            return BaseCommandRegistration.Instantiate(serviceProvider);
        }

        public bool IsEnabled
        {
            get
            {
                return BaseCommandRegistration.IsEnabled && (CommandData.Enabled ?? true);
            }
        }
    }
}