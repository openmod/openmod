using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using OpenMod.API.Plugins;

namespace OpenMod.Core.Plugins
{
    public class FileSystemPluginAssembliesSource : IPluginAssembliesSource
    {
        private readonly string m_PluginsDirectory;

        public FileSystemPluginAssembliesSource(string pluginsDirectory)
        {
            if (!Directory.Exists(pluginsDirectory))
            {
                Directory.CreateDirectory(pluginsDirectory);
            }

            m_PluginsDirectory = pluginsDirectory;
        }

        public virtual async Task<ICollection<Assembly>> LoadPluginAssembliesAsync()
        {
            var assemblyList = new List<Assembly>();
            foreach (var file in Directory.GetFiles(m_PluginsDirectory, "*.dll"))
            {
                using (FileStream stream = File.Open(file, FileMode.Open))
                {
                    byte[] data = new byte[stream.Length];
                    await stream.ReadAsync(data, 0, (int)stream.Length);
                    var pluginAssembly = Assembly.Load(data);

                    var pluginMetadata = pluginAssembly.GetCustomAttribute<PluginMetadataAttribute>();
                    if (pluginMetadata == null)
                    {
                        // not a plugin, but could be a library
                        continue;
                    }

                    assemblyList.Add(pluginAssembly);
                }
            }

            return assemblyList;
        }
    }
}