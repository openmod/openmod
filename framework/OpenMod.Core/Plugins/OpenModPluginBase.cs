using System;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Plugins;
using OpenMod.Core.Helpers;
using Semver;

namespace OpenMod.Core.Plugins
{
    public abstract class OpenModPluginBase : IOpenModPlugin, IDisposable
    {
        public string OpenModComponentId { get; }
        public string WorkingDirectory { get; }
        public bool IsComponentAlive { get; protected set; }
        public string DisplayName { get; }
        public string Author { get; }
        public SemVersion Version { get; }
        public IRuntime Runtime { get; }

        private readonly ILifetimeScope m_LifetimeScope;

        protected OpenModPluginBase(IServiceProvider serviceProvider)
        {
            m_LifetimeScope = serviceProvider.GetRequiredService<ILifetimeScope>();
            Configuration = serviceProvider.GetRequiredService<IConfiguration>();

            Runtime = serviceProvider.GetRequiredService<IRuntime>();

            var metadata = GetType().Assembly.GetCustomAttribute<PluginMetadataAttribute>();
            OpenModComponentId = metadata.Id;
            Version = metadata.Version;
            DisplayName = metadata.DisplayName;
            Author = metadata.Author;
            WorkingDirectory = PluginHelper.GetWorkingDirectory(Runtime, metadata.Id);
        }

        public IConfiguration Configuration { get; set; }

        public abstract Task LoadAsync();

        public abstract Task UnloadAsync();

        public void Dispose()
        {
            AsyncHelper.RunSync(UnloadAsync);
            m_LifetimeScope?.Dispose();
            OnDispose();
        }

        protected virtual void OnDispose()
        {

        }
    }
}