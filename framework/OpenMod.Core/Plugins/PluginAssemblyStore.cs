using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using OpenMod.API.Plugins;
using OpenMod.Core.Helpers;
using Serilog;

namespace OpenMod.Core.Plugins
{
    public class PluginAssemblyStore : IPluginAssemblyStore, IDisposable
    {
        public IReadOnlyCollection<PluginProvider> PluginProviders { get; }

        private List<WeakReference> m_LoadedPluginAssemblies;
        public IReadOnlyCollection<Assembly> LoadedPluginAssemblies
        {
            get
            {
                return m_LoadedPluginAssemblies
                    .Where(d => d.IsAlive)
                    .Select(d => d.Target)
                    .Cast<Assembly>()
                    .ToList();
            }
        }

        public PluginAssemblyStore(ICollection<PluginProvider> pluginProviders)
        {
            PluginProviders = pluginProviders.ToList();
        }

        public async Task LoadPluginAssembliesAsync()
        {
            var assemblies = new List<Assembly>();
            foreach (var pluginProvider in PluginProviders)
            {
                var providerAssemblies = await pluginProvider.LoadPluginAssembliesAsync();
                foreach (var providerAssembly in providerAssemblies.ToList())
                {
                    var pluginMetadata = providerAssembly.GetCustomAttribute<PluginMetadataAttribute>();
                    if (pluginMetadata == null)
                    {
                        Log.Debug($"No plugin metadata attribute found in assembly: {providerAssemblies}; skipping loading of this assembly as plugin");
                        providerAssemblies.Remove(providerAssembly);
                    }

                    if (!providerAssembly.FindTypes<IOpenModPlugin>(false).Any())
                    {
                        Log.Debug($"No IOpenModPlugin implementation found in assembly: {providerAssemblies}; skipping loading of this assembly as plugin");
                        providerAssemblies.Remove(providerAssembly);
                    }
                }

                assemblies.AddRange(providerAssemblies);
            }

            m_LoadedPluginAssemblies = assemblies.Select(d => new WeakReference(d)).ToList();
        }

        public void Dispose()
        {
            // Clear assembly references
            m_LoadedPluginAssemblies.Clear();

            foreach (var provider in PluginProviders)
            {
                if (provider is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}