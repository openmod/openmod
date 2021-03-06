using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.API.Permissions;
using OpenMod.API.Prioritization;
using OpenMod.Core.Ioc;
using OpenMod.Core.Permissions;

namespace OpenMod.Core.Commands
{
    [OpenModInternal]
    public class OpenModComponentBoundCommandRegistration : ICommandRegistration
    {
        public IOpenModComponent Component { get; }
        public string? Syntax { get; private set; }
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public Priority Priority { get; set; }
        public IReadOnlyCollection<IPermissionRegistration>? PermissionRegistrations { get; set; }
        public IReadOnlyCollection<string>? Aliases { get; set; }
        public string? ParentId { get; set; }
        public Type? CommandType { get; }
        public MethodInfo? CommandMethod { get; }

        private ICollection<Type>? m_CommandActorTypes;

        public OpenModComponentBoundCommandRegistration(IOpenModComponent component, Type commandType)
        {
            Component = component ?? throw new ArgumentNullException(nameof(component));
            CommandType = commandType ?? throw new ArgumentNullException(nameof(commandType));
            ReadAttributes(commandType);
        }

        public OpenModComponentBoundCommandRegistration(IOpenModComponent component, MethodInfo methodInfo)
        {
            Component = component ?? throw new ArgumentNullException(nameof(component));
            CommandMethod = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));
            ReadAttributes(methodInfo);
        }

        private void ReadAttributes(MemberInfo memberInfo)
        {
            if (memberInfo == null)
            {
                throw new ArgumentNullException(nameof(memberInfo));
            }

            var commandAttribute = memberInfo.GetCustomAttribute<CommandAttribute>();
            Name = commandAttribute.Name;
            Description = memberInfo.GetCustomAttribute<CommandDescriptionAttribute>()?.Description;
            Priority = commandAttribute.Priority;
            Aliases = memberInfo.GetCustomAttributes<CommandAliasAttribute>().Select(d => d.Alias).ToList();
            Syntax = memberInfo.GetCustomAttribute<CommandSyntaxAttribute>()?.Syntax;

            PermissionRegistrations = memberInfo.GetCustomAttributes<RegisterCommandPermissionAttribute>(true)
                .Select(d => new PermissionRegistration
                {
                    DefaultGrant = d.DefaultGrant,
                    Description = d.Description,
                    Owner = Component,
                    Permission = d.Permission
                })
                .ToList();

            Id = CommandType != null
                ? CommandType.FullName!
                : $"{CommandMethod!.DeclaringType!.FullName}.{CommandMethod!.Name}";

            m_CommandActorTypes = memberInfo.GetCustomAttributes<CommandActorAttribute>().Select(d => d.ActorType).ToList();

            var parent = memberInfo.GetCustomAttribute<CommandParentAttribute>();
            if (parent != null)
            {
                ParentId = parent.CommandType != null
                    ? parent.CommandType.FullName
                    : $"{parent.DeclaringType!.FullName}.{parent.MethodName}";
            }
        }

        public bool SupportsActor(ICommandActor actor)
        {
            if (actor == null)
            {
                throw new ArgumentNullException(nameof(actor));
            }

            if (m_CommandActorTypes == null || m_CommandActorTypes.Count == 0)
            {
                return true;
            }

            return m_CommandActorTypes.Any(d => d.IsInstanceOfType(actor));
        }

        public ICommand Instantiate(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            if (CommandType != null)
            {
                var lifetime = serviceProvider.GetRequiredService<ILifetimeScope>();
                return (ICommand)ActivatorUtilitiesEx.CreateInstance(lifetime, CommandType);
            }

            return new MethodCommandWrapper(CommandMethod!, serviceProvider);
        }

        public bool IsEnabled
        {
            get
            {
                return Component.IsComponentAlive;
            }
        }
    }
}