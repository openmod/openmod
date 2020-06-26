using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenMod.API.Commands;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using OpenMod.Core.Helpers;
using OpenMod.Core.Prioritization;

namespace OpenMod.Core.Commands
{
    [UsedImplicitly]
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class CommandStore : ICommandStore, IAsyncDisposable
    {
        private readonly PriorityComparer m_Comparer;
        private readonly IOptions<CommandStoreOptions> m_Options;
        private readonly IServiceProvider m_ServiceProvider;
        private IReadOnlyCollection<ICommandSource> m_CommandSources;

        public CommandStore(IOptions<CommandStoreOptions> options, IServiceProvider serviceProvider)
        {
            m_Comparer = new PriorityComparer(PriortyComparisonMode.HighestFirst);
            m_CommandSources = options.Value.CreateCommandSources(serviceProvider);
            m_Options = options;
            m_ServiceProvider = serviceProvider;
            options.Value.OnCommandSourcesChanged += OnCommandSourcesChanged;
        }

        private void OnCommandSourcesChanged()
        {
            AsyncHelper.RunSync(m_CommandSources.DisposeAllAsync);
            m_CommandSources = m_Options.Value.CreateCommandSources(m_ServiceProvider);
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
            m_Options.Value.OnCommandSourcesChanged -= OnCommandSourcesChanged;
        }

        public async ValueTask DisposeAsync()
        {
            await m_CommandSources.DisposeAllAsync();
        }
    }
}