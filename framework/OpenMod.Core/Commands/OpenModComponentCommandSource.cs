using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Autofac.Util;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.Common.Helpers;
using OpenMod.Core.Ioc;

namespace OpenMod.Core.Commands
{
    [OpenModInternal]
    public class OpenModComponentCommandSource : ICommandSource
    {

        private readonly ILogger m_Logger;
        private readonly IOpenModComponent m_OpenModComponent;
        private readonly List<ICommandRegistration> m_Commands;

        public OpenModComponentCommandSource(
            ILogger logger, 
            IOpenModComponent openModComponent, 
            Assembly assembly)
        {
            m_Logger = logger;
            m_OpenModComponent = openModComponent;
            m_Commands = new List<ICommandRegistration>();
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

                var registatration = new OpenModComponentBoundCommandRegistration(m_OpenModComponent, type);

                m_Commands.Add(registatration);
            }

            try
            {
                foreach (var type in assembly.GetLoadableTypes())
                {
                    ScanTypeForCommands(type);
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                foreach (var type in ex.Types)
                {
                    if (type == null)
                    {
                        continue;
                    }

                    ScanTypeForCommands(type);
                }
            }
        }

        private void ScanTypeForCommands(IReflect type)
        {
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                var commandAttribute = method.GetCustomAttribute<CommandAttribute>();
                if (commandAttribute == null)
                {
                    continue;
                }

                m_Commands.Add(new OpenModComponentBoundCommandRegistration(m_OpenModComponent, method));
            }
        }

        public string Id
        {
            get { return m_OpenModComponent.OpenModComponentId; }
        }

        public Task<IReadOnlyCollection<ICommandRegistration>> GetCommandsAsync()
        {
            return Task.FromResult<IReadOnlyCollection<ICommandRegistration>>(m_Commands);
        }
    }
}