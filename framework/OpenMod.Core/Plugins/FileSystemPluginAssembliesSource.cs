using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Plugins;
using OpenMod.Common.Helpers;
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
            foreach (var assemblyPath in Directory.GetFiles(m_PluginsDirectory, "*.dll"))
            {
                try
                {
                    var assemblyData = await FileHelper.ReadAllBytesAsync(assemblyPath);
                    var assemblySymbolsPath = Path.ChangeExtension(assemblyPath, "pdb");
                    var assemblySymbols = File.Exists(assemblySymbolsPath)
                        ? await FileHelper.ReadAllBytesAsync(assemblySymbolsPath)
                        : null;
                    var pluginAssembly = Hotloader.LoadAssembly(assemblyData, assemblySymbols);

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
                    m_Logger.LogWarning(ex, "Failed to load plugin metadata from file: {File}", assemblyPath);
                }
            }

            return assemblyList;
        }
    }
}
