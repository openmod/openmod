using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac.Util;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.Core.Helpers;

namespace OpenMod.Core.Commands
{
    public class OpenModComponentCommandSource : ICommandSource
    {
        private readonly IOpenModComponent m_OpenModComponent;

        public OpenModComponentCommandSource(IOpenModComponent openModComponent)
        {
            m_OpenModComponent = openModComponent;
            Commands = new List<ICommandRegistration>();
            ScanAssemblyForCommmmands(openModComponent.GetType().Assembly);
        }

        private void ScanAssemblyForCommmmands(Assembly assembly)
        {
            var types = assembly.GetTypesWithInterface<ICommand>(false);
            foreach (var type in types)
            {
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