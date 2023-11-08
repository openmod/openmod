using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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
            var assemblyTypes = assembly.FindAllTypes().ToList();
            var commandTypes = assemblyTypes.FindTypes<ICommand>();

            foreach (var type in commandTypes)
            {
                if (type.GetCustomAttribute<DontAutoRegister>(false) != null)
                {
                    continue;
                }

                var commandAttribute = type.GetCustomAttribute<CommandAttribute>();
                if (commandAttribute == null)
                {
                    m_Logger.LogWarning(
                        "Type {Type} is missing the [Command(string name)] attribute. Command will not be registered",
                        type);
                    continue;
                }

                var registatration = new OpenModComponentBoundCommandRegistration(m_OpenModComponent, type);

                m_Commands.Add(registatration);

            }

            foreach (var type in assemblyTypes)
            {
                ScanTypeForCommands(type);
            }
        }

        private void ScanTypeForCommands(IReflect type)
        {
            try
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
            catch(Exception ex)
            {
                m_Logger.LogDebug(ex, "Exception in ScanTypeForCommands for type: {Type}", type);
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