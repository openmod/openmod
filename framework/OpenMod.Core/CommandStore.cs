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
    public class CommandStore : ICommandStore, IDisposable
    {
        private readonly PriorityComparer m_Comparer;
        private readonly IDisposable m_OptionsOnChange;
        private IReadOnlyCollection<ICommandSource> m_CommandSources;

        public CommandStore(IOptionsMonitor<CommandStoreOptions> options, IServiceProvider serviceProvider)
        {
            m_Comparer = new PriorityComparer(PriortyComparisonMode.HighestFirst);
            m_CommandSources = options.CurrentValue.CreateCommandSources(serviceProvider);
            m_OptionsOnChange = options.OnChange((newOptions, name) =>
            {
                m_CommandSources = newOptions.CreateCommandSources(serviceProvider);
            });
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

        public void Dispose()
        {
            m_OptionsOnChange.Dispose();
        }
    }
}