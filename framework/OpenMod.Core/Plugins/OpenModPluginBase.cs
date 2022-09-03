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
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace OpenMod.Core.Plugins
{
    /// <summary>
    /// Base class for all OpenMod plugins.
    /// </summary>
    public abstract class OpenModPluginBase : IOpenModPlugin, IAsyncDisposable
    {
        public virtual string OpenModComponentId { get; }
        public virtual string WorkingDirectory { get; }
        public virtual bool IsComponentAlive { get; protected set; }
        public virtual PluginStatus Status { get; private set; }
        public virtual string DisplayName { get; }
        public virtual string? Author { get; }
        public virtual string? Website { get; }
        public virtual string? Description { get; }
        public virtual SemVersion Version { get; }
        public virtual IDataStore DataStore { get; }
        public virtual ILifetimeScope LifetimeScope { get; }
        public virtual IConfiguration Configuration { get; protected set; }
        public IRuntime Runtime { get; }
        public IEventBus EventBus { get; }
        protected ILogger Logger { get; set; }
        protected Harmony Harmony { get; private set; } = null!;

        private readonly IOptions<CommandStoreOptions> m_CommandStoreOptions;
        private OpenModComponentCommandSource m_CommandSource = null!;

        protected OpenModPluginBase(IServiceProvider serviceProvider)
        {
            LifetimeScope = serviceProvider.GetRequiredService<ILifetimeScope>();
            // ReSharper disable once VirtualMemberCallInConstructor
            Configuration = serviceProvider.GetRequiredService<IConfiguration>();
            DataStore = serviceProvider.GetRequiredService<IDataStore>();
            Runtime = serviceProvider.GetRequiredService<IRuntime>();
            EventBus = serviceProvider.GetRequiredService<IEventBus>();
            m_CommandStoreOptions = serviceProvider.GetRequiredService<IOptions<CommandStoreOptions>>();

            var loggerType = typeof(ILogger<>).MakeGenericType(GetType());
            Logger = (ILogger)serviceProvider.GetRequiredService(loggerType);

            Status = PluginStatus.Initialized;

            var metadata = GetType().Assembly.GetCustomAttribute<PluginMetadataAttribute>();
            OpenModComponentId = metadata.Id;
            // ReSharper disable once VirtualMemberCallInConstructor
            Version = GetPluginVersion();

            DisplayName = !string.IsNullOrEmpty(metadata.DisplayName)
                ? metadata.DisplayName!
                : metadata.Id;

            Author = metadata.Author;
            Website = metadata.Website;
            Description = metadata.Description;
            WorkingDirectory = PluginHelper.GetWorkingDirectory(Runtime, metadata.Id);
        }

        protected virtual SemVersion GetPluginVersion()
        {
            return VersionHelper.ParseAssemblyVersion(GetType().Assembly);
        }

        [OpenModInternal]
        public virtual Task LoadAsync()
        {
            // Only load plugin after initialization
            if (Status != PluginStatus.Initialized)
            {
                return Task.CompletedTask;
            }

            Status = PluginStatus.Loading;

            try
            {
                Logger.LogInformation("[loading] {DisplayName} v{Version}", DisplayName, Version);

                m_CommandSource = new OpenModComponentCommandSource(Logger, this, GetType().Assembly);
                m_CommandStoreOptions.Value.AddCommandSource(m_CommandSource);

                Harmony = new Harmony(OpenModComponentId);
                Harmony.PatchAll(GetType().Assembly);

                IsComponentAlive = true;

                EventBus.Subscribe(this, GetType().Assembly);

                Status = PluginStatus.Loaded;
            }
            catch
            {
                Status = PluginStatus.ExceptionWhenLoading;

                throw;
            }

            return Task.CompletedTask;
        }


        [OpenModInternal]
        public virtual Task UnloadAsync()
        {
            // Only unload after plugin loaded or attempted to load
            if (Status != PluginStatus.Loaded && Status != PluginStatus.ExceptionWhenLoading)
            {
                return Task.CompletedTask;
            }

            Status = PluginStatus.Unloading;

            try
            {
                Harmony.UnpatchAll(OpenModComponentId);

                m_CommandStoreOptions.Value.RemoveCommandSource(m_CommandSource);

                EventBus.Unsubscribe(this);

                IsComponentAlive = false;

                Status = PluginStatus.Unloaded;
                return Task.CompletedTask;
            }
            catch
            {
                Status = PluginStatus.ExceptionWhenUnloading;

                throw;
            }
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                if (!await OnDispose())
                {
                    await UnloadAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception occurred when disposing plugin '{ComponentId}'", OpenModComponentId);
            }

            await EventBus.EmitAsync(this, this, new PluginDisposedEvent(this));
        }

        protected virtual ValueTask<bool> OnDispose()
        {
            return new ValueTask<bool>(false);
        }
    }
}