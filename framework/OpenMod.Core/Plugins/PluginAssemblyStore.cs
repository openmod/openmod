using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Plugins;
using OpenMod.Core.Helpers;
using Serilog;

namespace OpenMod.Core.Plugins
{
    [OpenModInternal]
    public class PluginAssemblyStore : IPluginAssemblyStore, IDisposable
    {
        private readonly ILogger<PluginAssemblyStore> m_Logger;

        public PluginAssemblyStore(ILogger<PluginAssemblyStore> logger)
        {
            m_Logger = logger;
        }

        private readonly List<WeakReference> m_LoadedPluginAssemblies = new List<WeakReference>();
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

        public async Task<ICollection<Assembly>> LoadPluginAssembliesAsync(IPluginAssembliesSource source)
        {
            var providerAssemblies = await source.LoadPluginAssembliesAsync();
            foreach (var providerAssembly in providerAssemblies.ToList())
            {
                var pluginMetadata = providerAssembly.GetCustomAttribute<PluginMetadataAttribute>();
                if (pluginMetadata == null)
                {
                    m_Logger.LogWarning($"No plugin metadata attribute found in assembly: {providerAssembly}; skipping loading of this assembly as plugin");
                    providerAssemblies.Remove(providerAssembly);
                    continue;
                }

                ICollection<Type> types;
                try
                {
                    types = providerAssembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    m_Logger.LogTrace(ex, $"Failed to load some types from plugin \"{pluginMetadata.Id}\"");
                    if (ex.LoaderExceptions != null && ex.LoaderExceptions.Length > 0)
                    {
                        foreach (var loaderException in ex.LoaderExceptions)
                        {
                            m_Logger.LogTrace(loaderException, "Loader Exception: ");
                        }
                    }

                    types = ex.Types.Where(d => d != null).ToArray();
                }

                if (!types.Any(d => typeof(IOpenModPlugin).IsAssignableFrom(d) && !d.IsAbstract && d.IsClass))
                {
                    m_Logger.LogWarning($"No {nameof(IOpenModPlugin)} implementation found in assembly: {providerAssembly}; skipping loading of this assembly as plugin");
                    providerAssemblies.Remove(providerAssembly);
                    continue;
                }
            }

            m_LoadedPluginAssemblies.AddRange(providerAssemblies.Select(d => new WeakReference(d)));
            return providerAssemblies;
        }

        public void Dispose()
        {
            // Clear assembly references
            m_LoadedPluginAssemblies.Clear();
        }
    }
}