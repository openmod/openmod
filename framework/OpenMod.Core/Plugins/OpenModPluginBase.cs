using System;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenMod.API;
using OpenMod.API.Persistence;
using OpenMod.API.Plugins;
using OpenMod.Core.Commands;
using Semver;

namespace OpenMod.Core.Plugins
{
    public abstract class OpenModPluginBase : IOpenModPlugin, IAsyncDisposable
    {
        public string OpenModComponentId { get; }
        public string WorkingDirectory { get; }
        public bool IsComponentAlive { get; protected set; }
        public string DisplayName { get; }
        public string Author { get; }
        public SemVersion Version { get; }
        public IRuntime Runtime { get; }
        public IDataStore DataStore { get; }
        public ILifetimeScope LifetimeScope { get; }
        public IConfiguration Configuration { get; set; }

        private readonly IOptions<CommandStoreOptions> m_CommandStoreOptions;
        private readonly ILoggerFactory m_LoggerFactory;
        private OpenModComponentCommandSource m_CommandSource;

        protected OpenModPluginBase(IServiceProvider serviceProvider)
        {
            LifetimeScope = serviceProvider.GetRequiredService<ILifetimeScope>();
            Configuration = serviceProvider.GetRequiredService<IConfiguration>();
            DataStore = serviceProvider.GetRequiredService<IDataStore>();
            Runtime = serviceProvider.GetRequiredService<IRuntime>();
            m_LoggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            m_CommandStoreOptions = serviceProvider.GetRequiredService<IOptions<CommandStoreOptions>>();
            
            var metadata = GetType().Assembly.GetCustomAttribute<PluginMetadataAttribute>();
            OpenModComponentId = metadata.Id;
            Version = metadata.Version;
            DisplayName = metadata.DisplayName;
            Author = metadata.Author;
            WorkingDirectory = PluginHelper.GetWorkingDirectory(Runtime, metadata.Id);
        }

        public virtual Task LoadAsync()
        {
            var logger = m_LoggerFactory.CreateLogger<OpenModComponentCommandSource>();
            m_CommandSource = new OpenModComponentCommandSource(logger, this);
            m_CommandStoreOptions.Value.AddCommandSource(m_CommandSource);

            return Task.CompletedTask;
        }

        public virtual Task UnloadAsync()
        {
            m_CommandStoreOptions.Value.RemoveCommandSource(m_CommandSource);
            return Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            if (!await OnDispose())
            {
                await UnloadAsync();
            }

            LifetimeScope?.Dispose();
        }

        protected virtual ValueTask<bool> OnDispose()
        {
            return new ValueTask<bool>(false);
        }
    }
}