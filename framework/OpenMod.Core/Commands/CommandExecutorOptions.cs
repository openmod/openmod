using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Commands;
using OpenMod.API.Permissions;
using OpenMod.Core.Helpers.Prioritization;
using OpenMod.Core.Prioritization;

namespace OpenMod.Core.Commands
{
    public class CommandExecutorOptions
    {
        private readonly List<Type> m_CommandSourceTypes;
        private readonly List<ICommandSource> m_CommandSources;
        private readonly PriorityComparer m_PriorityComparer;

        public CommandExecutorOptions()
        {
            m_CommandSourceTypes = new List<Type>();
            m_CommandSources = new List<ICommandSource>();
            m_PriorityComparer = new PriorityComparer(PriortyComparisonMode.HighestFirst);
        }

        public IReadOnlyCollection<ICommandSource> CreateCommandSources(IServiceProvider serviceProvider)
        {
            var sources = new List<ICommandSource>();
            sources.AddRange(m_CommandSources);
            foreach (var type in m_CommandSourceTypes)
            {
                sources.Add((ICommandSource) ActivatorUtilities.CreateInstance(serviceProvider, type));
            }

            return sources;
        }

        public void AddCommandSource(ICommandSource commandSource)
        {
            m_CommandSources.Add(commandSource);
        }

        public void RemoveCommandSource(ICommandSource commandSource)
        {
            m_CommandSources.Remove(commandSource);
        }

        public void AddCommandSource<TSource>() where TSource: ICommandSource
        {
            AddCommandSource(typeof(TSource));
        }

        public void AddCommandSource(Type type)
        {
            if (!typeof(IPermissionCheckProvider).IsAssignableFrom(type))
            {
                throw new Exception($"Type {type} must be an instance of IPermissionCheckProvider!");
            }

            if (m_CommandSourceTypes.Contains(type))
            {
                return;
            }

            m_CommandSourceTypes.Add(type);
            m_CommandSourceTypes.Sort((a, b) => m_PriorityComparer.Compare(a.GetPriority(), b.GetPriority()));
        }

        public void RemoveCommandSource<TSource>() where TSource : ICommandSource
        {
            RemoveCommandSource(typeof(TSource));
        }

        public bool RemoveCommandSource(Type type)
        {
            return m_CommandSourceTypes.RemoveAll(d => d == type) > 0;
        }
    }
}