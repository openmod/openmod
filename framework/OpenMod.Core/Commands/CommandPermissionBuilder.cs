using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using OpenMod.Core.Helpers;

namespace OpenMod.Core.Commands
{
    [OpenModInternal]
    [UsedImplicitly]
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class CommandPermissionBuilder : ICommandPermissionBuilder
    {
        private readonly Lazy<ICommandStore> m_CommandStore;

        public CommandPermissionBuilder(Lazy<ICommandStore> commandStore)
        {
            m_CommandStore = commandStore;
        }

        private readonly Dictionary<string, string> m_Cache = new();
        public virtual string GetPermission(ICommandRegistration registration)
        {
            return m_Cache.TryGetValue(registration.Id, out var cachedValue)
                ? cachedValue
                : GetPermission(registration, AsyncHelper.RunSync(() => m_CommandStore.Value.GetCommandsAsync()));
        }

        public virtual string GetPermission(ICommandRegistration registration, IReadOnlyCollection<ICommandRegistration> commands)
        {
            if (m_Cache.TryGetValue(registration.Id, out var cachedValue))
            {
                return cachedValue;
            }

            var commandPath = new StringBuilder(registration.Name);

            /* do reverse traversal from child to parent to build permission */
            var current = registration;
            while ((current = GetParentCommand(current, commands)) != null)
            {
                commandPath.Insert(0, current.Name + ".");
            }

            var permission = $"{registration.Component.OpenModComponentId}:commands.{commandPath}";

            m_Cache.Add(registration.Id, permission);
            return permission;
        }

        private ICommandRegistration? GetParentCommand(ICommandRegistration registration, IEnumerable<ICommandRegistration> commands)
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