using System;
using System.Collections.Generic;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Commands;
using OpenMod.Core.Ioc;
using OpenMod.Core.Prioritization;

namespace OpenMod.Core.Commands
{
    public class CommandStoreOptions
    {
        public delegate void CommandSourcesChanged();
        public event CommandSourcesChanged? OnCommandSourcesChanged;

        private readonly List<Type> m_CommandSourceTypes;
        private readonly List<ICommandSource> m_CommandSources;
        private readonly PriorityComparer m_PriorityComparer;

        public CommandStoreOptions()
        {
            m_CommandSourceTypes = new List<Type>();
            m_CommandSources = new List<ICommandSource>();
            m_PriorityComparer = new PriorityComparer(PriortyComparisonMode.HighestFirst);
        }

        public IReadOnlyCollection<ICommandSource> CreateCommandSources(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            var lifetime = serviceProvider.GetService<ILifetimeScope>();
            var sources = new List<ICommandSource>();
            sources.AddRange(m_CommandSources);

            foreach (var type in m_CommandSourceTypes)
            {
                sources.Add((ICommandSource)ActivatorUtilitiesEx.CreateInstance(lifetime!, type));
            }

            return sources;
        }

        public void AddCommandSource(ICommandSource commandSource)
        {
            if (commandSource == null)
            {
                throw new ArgumentNullException(nameof(commandSource));
            }

            m_CommandSources.Add(commandSource);
            OnCommandSourcesChanged?.Invoke();
        }

        public void RemoveCommandSource(ICommandSource commandSource)
        {
            if (commandSource == null)
            {
                throw new ArgumentNullException(nameof(commandSource));
            }

            m_CommandSources.Remove(commandSource);
            OnCommandSourcesChanged?.Invoke();
        }

        public void AddCommandSource<TSource>() where TSource : ICommandSource
        {
            AddCommandSource(typeof(TSource));
        }

        public void AddCommandSource(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!typeof(ICommandSource).IsAssignableFrom(type))
            {
                throw new Exception($"Type {type} must be an instance of ICommandSource!");
            }

            if (m_CommandSourceTypes.Contains(type))
            {
                return;
            }

            m_CommandSourceTypes.Add(type);
            m_CommandSourceTypes.Sort((a, b) => m_PriorityComparer.Compare(a.GetPriority(), b.GetPriority()));
            OnCommandSourcesChanged?.Invoke();
        }

        public void RemoveCommandSource<TSource>() where TSource : ICommandSource
        {
            RemoveCommandSource(typeof(TSource));
        }

        public bool RemoveCommandSource(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var result = m_CommandSourceTypes.RemoveAll(d => d == type) > 0;
            OnCommandSourcesChanged?.Invoke();
            return result;
        }
    }
}