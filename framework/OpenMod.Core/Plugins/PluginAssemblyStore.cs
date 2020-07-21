using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMod.API.Plugins;

namespace OpenMod.Core.Plugins
{
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
                    var missingTypes = new List<string>();
                    for (var i = 0; i < ex.LoaderExceptions.Length; i += 2)
                    {
                        if (i + 1 >= ex.LoaderExceptions.Length)
                            break;

                        if (!(ex.LoaderExceptions[i] is TypeLoadException &&
                              ex.LoaderExceptions[i + 1] is FileNotFoundException fileEx))
                        {
                            i--;
                            continue;
                        }

                        var packageName = fileEx.FileName.Split(',')[0];
                        if (!missingTypes.Contains(packageName))
                            missingTypes.Add(packageName);
                    }

                    if (missingTypes.Count > 0)
                    {
                        m_Logger.LogInformation($"Some libraries are missing for plugin \"{pluginMetadata.Id}\"");
                        m_Logger.LogInformation($"Missing type: {string.Join(", ", missingTypes)}.");
                        m_Logger.LogInformation("Try install from: \"openmod install 'type'\"");
                        providerAssemblies.Remove(providerAssembly);
                        continue;
                    }

                    m_Logger.LogInformation(ex, $"Failed to load some types from plugin \"{pluginMetadata.Id}\"");
                    if (ex.LoaderExceptions != null && ex.LoaderExceptions.Length > 0)
                    {
                        foreach (var loaderException in ex.LoaderExceptions)
                        {
                            m_Logger.LogInformation(loaderException, "Loader Exception: ");
                        }
                    }

                    types = ex.Types.Where(d => d != null).ToArray();
                }

                if (types.Any(d => typeof(IOpenModPlugin).IsAssignableFrom(d) && !d.IsAbstract && d.IsClass)) 
                    continue;

                m_Logger.LogWarning($"No {nameof(IOpenModPlugin)} implementation found in assembly: {providerAssembly}; skipping loading of this assembly as plugin");
                providerAssemblies.Remove(providerAssembly);
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