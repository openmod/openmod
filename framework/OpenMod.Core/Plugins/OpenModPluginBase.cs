using System;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using HarmonyLib;
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
using OpenMod.Core.Plugins.Events;
using Semver;

namespace OpenMod.Core.Plugins
{
    /// <summary>
    /// Base class for all OpenMod plugins.
    /// </summary>
    public abstract class OpenModPluginBase : IOpenModPlugin, IAsyncDisposable
    {
        private readonly IServiceProvider m_ServiceProvider;
        public virtual string OpenModComponentId { get; }
        public virtual string WorkingDirectory { get; }
        public virtual bool IsComponentAlive { get; protected set; }
        public virtual string DisplayName { get; }
        public virtual string? Author { get; }
        public virtual string? Website { get; }
        public virtual SemVersion Version { get; }
        public virtual IDataStore DataStore { get; }
        public virtual ILifetimeScope LifetimeScope { get; }
        public virtual IConfiguration Configuration { get; protected set; }
        public IRuntime Runtime { get; }
        public IEventBus EventBus { get; }
        protected ILogger Logger { get; set; } = null!;
        protected Harmony Harmony { get; private set; } = null!;
        private readonly IOptions<CommandStoreOptions> m_CommandStoreOptions;
        private readonly ILoggerFactory m_LoggerFactory;
        private OpenModComponentCommandSource m_CommandSource = null!;

        protected OpenModPluginBase(IServiceProvider serviceProvider)
        {
            m_ServiceProvider = serviceProvider;
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
                ? metadata.DisplayName!
                : metadata.Id;

            Author = metadata.Author;
            Website = metadata.Website;
            WorkingDirectory = PluginHelper.GetWorkingDirectory(Runtime, metadata.Id);
        }

        protected virtual SemVersion GetPluginVersion()
        {
            return VersionHelper.ParseAssemblyVersion(GetType().Assembly);
        }

        [OpenModInternal]
        public virtual Task LoadAsync()
        {
            Logger = m_LoggerFactory.CreateLogger(GetType());
            Logger.LogInformation("[loading] {DisplayName} v{Version}", DisplayName, Version);

            m_CommandSource = new OpenModComponentCommandSource(Logger, this, GetType().Assembly);
            m_CommandStoreOptions.Value.AddCommandSource(m_CommandSource);

            Harmony = new Harmony(OpenModComponentId);
            Harmony.PatchAll(GetType().Assembly);

            IsComponentAlive = true;

            EventBus.Subscribe(this, GetType().Assembly);

            return Task.CompletedTask;
        }


        [OpenModInternal]
        public virtual async Task UnloadAsync()
        {
            Harmony.UnpatchAll(OpenModComponentId);
            m_CommandStoreOptions.Value.RemoveCommandSource(m_CommandSource);
            EventBus.Unsubscribe(this);
            IsComponentAlive = false;

            if(Logger is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync();
            }
            else if(Logger is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (!await OnDispose())
            {
                await UnloadAsync();
            }

            await EventBus.EmitAsync(this, this, new PluginDisposedEvent(this));
        }

        protected virtual ValueTask<bool> OnDispose()
        {
            return new ValueTask<bool>(false);
        }
    }
}