using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenMod.API.Commands;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using OpenMod.Core.Prioritization;

namespace OpenMod.Core
{
    [UsedImplicitly]
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class CommandStore : ICommandStore
    {
        private readonly IReadOnlyCollection<ICommandSource> m_CommandSources;
        private readonly PriorityComparer m_Comparer;

        public CommandStore(IOptions<CommandStoreOptions> options, IServiceProvider serviceProvider)
        {
            m_Comparer = new PriorityComparer(PriortyComparisonMode.HighestFirst);
            m_CommandSources = options.Value.CreateCommandSources(serviceProvider);
        }

        public IReadOnlyCollection<ICommandRegistration> Commands
        {
            get
            {
                return m_CommandSources
                    .SelectMany(d => d.Commands)
                    .Where(d => d.Component.IsComponentAlive)
                    .OrderBy(d => d.Priority, m_Comparer)
                    .ToList();
            }
        }
    }
}