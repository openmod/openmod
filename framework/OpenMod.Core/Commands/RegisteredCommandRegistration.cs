using System;
using System.Collections.Generic;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.API.Permissions;
using OpenMod.API.Prioritization;

namespace OpenMod.Core.Commands
{
    internal class RegisteredCommandRegistration : ICommandRegistration
    {
        private readonly ICommandRegistration m_CommandRegistration;

        public RegisteredCommandRegistration(ICommandRegistration commandRegistration)
        {
            m_CommandRegistration = commandRegistration;
            Name = commandRegistration.Name;
            Priority = commandRegistration.Priority;
            Aliases = commandRegistration.Aliases;
            IsEnabled = true;
        }

        public IOpenModComponent Component => m_CommandRegistration.Component;

        public string Id => m_CommandRegistration.Id;

        public string Name { get; set; }

        public IReadOnlyCollection<string> Aliases { get; set; }

        public IReadOnlyCollection<IPermissionRegistration> PermissionRegistrations  => m_CommandRegistration.PermissionRegistrations;

        public string Description => m_CommandRegistration.Description;

        public string Syntax => m_CommandRegistration.Syntax;

        public Priority Priority { get; set; }

        public string ParentId => m_CommandRegistration.ParentId;

        public bool IsEnabled { get; set; }

        public Dictionary<string, object> Data { get; set; }

        public bool SupportsActor(ICommandActor actor)
        {
            return m_CommandRegistration.SupportsActor(actor);
        }

        public ICommand Instantiate(IServiceProvider serviceProvider)
        {
            return m_CommandRegistration.Instantiate(serviceProvider);
        }
    }
}