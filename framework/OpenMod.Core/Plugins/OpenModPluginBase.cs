using System;
using System.Reflection;
using System.Threading.Tasks;
using OpenMod.API.Plugins;
using OpenMod.Core.Helpers;
using Semver;

namespace OpenMod.Core.Plugins
{
    public abstract class OpenModPluginBase : IOpenModPlugin, IDisposable
    {
        public string OpenModComponentId { get; }
        public bool IsComponentAlive { get; protected set; }
        public string DisplayName { get; }
        public string Author { get; }
        public SemVersion Version { get; }

        protected OpenModPluginBase()
        {
            var metadata = GetType().Assembly.GetCustomAttribute<PluginMetadataAttribute>();
            OpenModComponentId = metadata.Id;
            Version = metadata.Version;
            DisplayName = metadata.DisplayName;
            Author = metadata.Author;
        }

        public abstract Task LoadAsync();

        public abstract Task UnloadAsync();

        public void Dispose()
        {
            AsyncHelper.RunSync(UnloadAsync);
        }
    }
}