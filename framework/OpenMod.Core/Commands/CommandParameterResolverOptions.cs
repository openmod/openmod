using System;
using System.Collections.Generic;
using OpenMod.API.Commands;
using OpenMod.Core.Prioritization;

namespace OpenMod.Core.Commands
{
    public class CommandParameterResolverOptions
    {
        private readonly List<Type> m_CommandParameterResolveProviderTypes;
        private readonly PriorityComparer m_PriorityComparer;

        public CommandParameterResolverOptions()
        {
            m_CommandParameterResolveProviderTypes = new List<Type>();
            m_PriorityComparer = new PriorityComparer(PriortyComparisonMode.HighestFirst);
        }

        public IReadOnlyCollection<Type> CommandParameterResolverTypes
        {
            get { return m_CommandParameterResolveProviderTypes; }
        }

        public void AddCommandParameterResolveProvider(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!typeof(ICommandParameterResolveProvider).IsAssignableFrom(type))
            {
                throw new ArgumentException($"Type {type} must be an instance of {nameof(ICommandParameterResolveProvider)}!", nameof(type));
            }

            if (m_CommandParameterResolveProviderTypes.Contains(type))
            {
                return;
            }

            m_CommandParameterResolveProviderTypes.Add(type);
            m_CommandParameterResolveProviderTypes.Sort((a, b) => m_PriorityComparer.Compare(a.GetPriority(), b.GetPriority()));
        }

        public bool RemoveCommandParameterResolveProvider(Type type)
        {
            return m_CommandParameterResolveProviderTypes.RemoveAll(d => d == type) > 0;
        }
    }
}
