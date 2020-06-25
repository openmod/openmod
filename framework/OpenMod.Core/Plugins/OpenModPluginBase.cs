using System;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Persistence;
using OpenMod.API.Plugins;
using OpenMod.Core.Commands;
using OpenMod.Core.Helpers;
using Semver;

namespace OpenMod.Core.Plugins
{
    public abstract class OpenModPluginBase : IOpenModPlugin, IAsyncDisposable
    {
        public virtual string OpenModComponentId { get; }
        public virtual string WorkingDirectory { get; }
        public virtual bool IsComponentAlive { get; protected set; }
        public virtual string DisplayName { get; }
        public virtual string Author { get; }
        public virtual SemVersion Version { get; }
        public virtual IDataStore DataStore { get; }
        public virtual ILifetimeScope LifetimeScope { get; }
        public virtual IConfiguration Configuration { get; protected set; }
        public IRuntime Runtime { get; }
        public IEventBus EventBus { get; }
        protected ILogger Logger { get; set; }

        private readonly IOptions<CommandStoreOptions> m_CommandStoreOptions;
        private readonly ILoggerFactory m_LoggerFactory;
        private OpenModComponentCommandSource m_CommandSource;

        protected OpenModPluginBase(IServiceProvider serviceProvider)
        {
            LifetimeScope = serviceProvider.GetRequiredService<ILifetimeScope>();
            Configuration = serviceProvider.GetRequiredService<IConfiguration>();
            DataStore = serviceProvider.GetRequiredService<IDataStore>();
            Runtime = serviceProvider.GetRequiredService<IRuntime>();
            EventBus = serviceProvider.GetRequiredService<IEventBus>();
            m_LoggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            m_CommandStoreOptions = serviceProvider.GetRequiredService<IOptions<CommandStoreOptions>>();
            
            var metadata = GetType().Assembly.GetCustomAttribute<PluginMetadataAttribute>();
            OpenModComponentId = metadata.Id;
            Version = GetPluginVersion();

            DisplayName = !string.IsNullOrEmpty(metadata.DisplayName)
                ? metadata.DisplayName
                : GetType().Name;

            Author = metadata.Author;
            WorkingDirectory = PluginHelper.GetWorkingDirectory(Runtime, metadata.Id);
        }

        protected virtual SemVersion GetPluginVersion()
        {
            return VersionHelper.ParseAssemblyVersion(GetType().Assembly);
        }

        public virtual Task LoadAsync()
        {
            Logger = m_LoggerFactory.CreateLogger(GetType());
            Logger.LogInformation($"[loading] {DisplayName} v{Version}");
            m_CommandSource = new OpenModComponentCommandSource(Logger, this);
            m_CommandStoreOptions.Value.AddCommandSource(m_CommandSource);
            IsComponentAlive = true;

            EventBus.Subscribe(this, GetType().Assembly);

            return Task.CompletedTask;
        }


        public virtual Task UnloadAsync()
        {
            m_CommandStoreOptions.Value.RemoveCommandSource(m_CommandSource);
            EventBus.Unsubscribe(this);
            IsComponentAlive = false;

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