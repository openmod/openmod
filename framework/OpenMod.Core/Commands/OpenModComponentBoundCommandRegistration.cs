using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.API.Prioritization;

namespace OpenMod.Core.Commands
{
    public class OpenModComponentBoundCommandRegistration : ICommandRegistration
    {
        private Type[] m_CommandActorTypes;
        public IOpenModComponent Component { get; }
        public Type CommandType { get; }
        public MethodInfo CommandMethod { get; }

        public OpenModComponentBoundCommandRegistration(IOpenModComponent component, Type commandType)
        {
            Component = component;
            CommandType = commandType;
            ReadAttributes(commandType);
        }

        public OpenModComponentBoundCommandRegistration(IOpenModComponent component, MethodInfo methodInfo)
        {
            Component = component;
            CommandMethod = methodInfo;
            ReadAttributes(methodInfo);
        }

        private void ReadAttributes(MemberInfo memberInfo)
        {
            var commandAttribute = memberInfo.GetCustomAttribute<CommandAttribute>();
            Name = commandAttribute.Name;
            Summary = memberInfo.GetCustomAttribute<CommandSummaryAttribute>()?.Summary;
            Description = memberInfo.GetCustomAttribute<CommandSummaryAttribute>()?.Summary;
            Priority = commandAttribute.Priority;
            Aliases = memberInfo.GetCustomAttributes<CommandAliasAttribute>().Select(d => d.Alias).ToList();
            Syntax = memberInfo.GetCustomAttribute<CommandSyntaxAttribute>()?.Syntax;

            if (CommandType != null)
            {
                // class based command
                Id = CommandType.FullName;
            }
            else
            {
                // method based command
                Id = $"{CommandMethod.DeclaringType.FullName}.{CommandMethod.Name}";
            }

            m_CommandActorTypes = memberInfo.GetCustomAttribute<CommandActorAttribute>()?.CommandActorTypes ?? new[] {typeof(ICommandActor)};

            var parent = memberInfo.GetCustomAttribute<CommandParentAttribute>();
            if (parent != null)
            {
                if (parent.CommandType != null)
                {
                    ParentId = parent.CommandType.FullName;
                }
                else
                {
                    ParentId = $"{parent.DeclaringType.FullName}.{parent.MethodName}";
                }
            }

            Permission = Id;
        }

        public ILifetimeScope OwnerLifetimeScope
        {
            get { return Component.LifetimeScope; }
        }

        public string Syntax { get; private set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public Priority Priority { get; set; }
        public IReadOnlyCollection<string> Aliases { get; set; }
        public string ParentId { get; set; }
        public string Permission { get; set; }

        public bool SupportsActor(ICommandActor actor)
        {
            return m_CommandActorTypes.Any(d => d.IsInstanceOfType(actor));
        }

        public ICommand Instantiate(IServiceProvider serviceProvider)
        {
            if (CommandType != null)
            {
                return (ICommand)ActivatorUtilities.CreateInstance(serviceProvider, CommandType);
            }

            return new MethodCommandWrapper(CommandMethod, serviceProvider);
        }
    }
}