using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Plugins;
using OpenMod.Common.Hotloading;

namespace OpenMod.Core.Plugins
{
    [OpenModInternal]
    public class FileSystemPluginAssembliesSource : IPluginAssembliesSource
    {
        private readonly ILogger m_Logger;
        private readonly string m_PluginsDirectory;
        public FileSystemPluginAssembliesSource(ILogger logger, string pluginsDirectory)
        {
            if (!Directory.Exists(pluginsDirectory))
            {
                Directory.CreateDirectory(pluginsDirectory);
            }

            m_Logger = logger;
            m_PluginsDirectory = pluginsDirectory;
        }

        public virtual async Task<ICollection<Assembly>> LoadPluginAssembliesAsync()
        {
            var assemblyList = new List<Assembly>();
            foreach (var file in Directory.GetFiles(m_PluginsDirectory, "*.dll"))
            {
                try
                {
                    using var stream = File.Open(file, FileMode.Open);
                    var data = new byte[stream.Length];
                    await stream.ReadAsync(data, 0, (int) stream.Length);
                    var pluginAssembly = Hotloader.LoadAssembly(data);

                    var pluginMetadata = pluginAssembly.GetCustomAttribute<PluginMetadataAttribute>();
                    if (pluginMetadata == null)
                    {
                        // not a plugin, but could be a library
                        continue;
                    }

                    assemblyList.Add(pluginAssembly);
                }
                catch (Exception ex)
                {
                    m_Logger.LogWarning(ex, "Failed to load plugin metadata from file: {File}", file);
                }
            }

            return assemblyList;
        }
    }
}