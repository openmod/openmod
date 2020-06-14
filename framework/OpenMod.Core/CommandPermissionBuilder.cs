using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Commands;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;

namespace OpenMod.Core
{
    [UsedImplicitly]
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class CommandPermissionBuilder : ICommandPermissionBuilder
    {
        private readonly ICommandStore m_CommandStore;

        public CommandPermissionBuilder(ICommandStore commandStore)
        {
            m_CommandStore = commandStore;
        }

        private readonly Dictionary<string, string> m_Cache = new Dictionary<string, string>();
        public virtual string GetPermission(ICommandRegistration registration)
        {
            return m_Cache.TryGetValue(registration.Id, out var cachedValue) ? cachedValue : GetPermission(registration, m_CommandStore.Commands);
        }

        public virtual string GetPermission(ICommandRegistration registration, IReadOnlyCollection<ICommandRegistration> commands)
        {
            // todo: read the commands file and get permission from there if exists; otherwise build it

            if (m_Cache.TryGetValue(registration.Id, out var cachedValue))
            {
                return cachedValue;
            }

            var permission = registration.Name;
            
            /* do reverse traversal from child to parent to build permission */
            var current = registration;
            while ((current = GetParentCommand(current, commands)) != null)
            {
                permission = current.Name + "." + permission;
            }

            m_Cache.Add(registration.Id, permission);
            return permission;
        }


        private ICommandRegistration GetParentCommand(ICommandRegistration registration, IEnumerable<ICommandRegistration> commands)
        {
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (registration.ParentId == null)
            {
                return null;
            }

            return commands.FirstOrDefault(d => d.Id.Equals(registration.ParentId, StringComparison.OrdinalIgnoreCase));
        }
    }
}