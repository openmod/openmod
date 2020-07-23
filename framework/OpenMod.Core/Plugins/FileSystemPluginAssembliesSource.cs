using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMod.API.Plugins;

namespace OpenMod.Core.Plugins
{
    public class FileSystemPluginAssembliesSource : IPluginAssembliesSource, IDisposable
    {
        private readonly ILogger m_Logger;
        private readonly string m_PluginsDirectory;
        private bool m_AssemblyResolverInstalled;

        public FileSystemPluginAssembliesSource(ILogger logger, string pluginsDirectory)
        {
            if (!Directory.Exists(pluginsDirectory))
            {
                Directory.CreateDirectory(pluginsDirectory);
            }

            m_Logger = logger;
            m_PluginsDirectory = pluginsDirectory;

            if (m_AssemblyResolverInstalled)
            {
                return;
            }

            /*AppDomain.CurrentDomain.AssemblyResolve += OnAsssemlbyResolve;*/
            m_AssemblyResolverInstalled = true;
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
                    var pluginAssembly = Assembly.Load(data);

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
                    m_Logger.LogWarning(ex, $"Failed to load plugin metadata from file: {file}");
                }
            }

            return assemblyList;
        }

        private Assembly OnAsssemlbyResolve(object sender, ResolveEventArgs args)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (!m_AssemblyResolverInstalled) return;
            /*AppDomain.CurrentDomain.AssemblyResolve -= OnAsssemlbyResolve;*/
            m_AssemblyResolverInstalled = false;
        }
    }
}