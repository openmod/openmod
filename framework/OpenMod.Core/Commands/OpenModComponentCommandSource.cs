using System.Collections.Generic;
using System.Reflection;
using Autofac.Util;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.Core.Helpers;
using OpenMod.Core.Ioc;

namespace OpenMod.Core.Commands
{
    public class OpenModComponentCommandSource : ICommandSource
    {
        private readonly ILogger m_Logger;
        private readonly IOpenModComponent m_OpenModComponent;

        public OpenModComponentCommandSource(ILogger logger, IOpenModComponent openModComponent) : this(logger, openModComponent, openModComponent.GetType().Assembly)
        {

        }

        public OpenModComponentCommandSource(ILogger logger, IOpenModComponent openModComponent, Assembly assembly)
        {
            m_Logger = logger;
            m_OpenModComponent = openModComponent;
            Commands = new List<ICommandRegistration>();
            ScanAssemblyForCommmmands(assembly);
        }

        private void ScanAssemblyForCommmmands(Assembly assembly)
        {
            var types = assembly.FindTypes<ICommand>();
            foreach (var type in types)
            {
                if (type.GetCustomAttribute<DontAutoRegister>(false) != null)
                {
                    continue;
                }

                var commandAttribute = type.GetCustomAttribute<CommandAttribute>();
                if (commandAttribute == null)
                {
                    m_Logger.LogWarning($"Type {type} is missing the [Command(string name)] attribute. Command will not be registered.");
                    continue;
                }

                Commands.Add(new OpenModComponentBoundCommandRegistration(m_OpenModComponent, type));
            }

            foreach (var type in assembly.GetLoadableTypes())
            {
                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                {
                    var commandAttribute = method.GetCustomAttribute<CommandAttribute>();
                    if (commandAttribute == null)
                    {
                        continue;
                    }

                    Commands.Add(new OpenModComponentBoundCommandRegistration(m_OpenModComponent, method));
                }
            }
        }

        public string Id
        {
            get { return m_OpenModComponent.OpenModComponentId; }
        }

        public ICollection<ICommandRegistration> Commands { get; }
    }
}