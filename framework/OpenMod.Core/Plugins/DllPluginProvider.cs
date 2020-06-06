using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace OpenMod.Core.Plugins
{
    public class DllPluginProvider : PluginProvider
    {
        private readonly string m_PluginsDirectory;

        public DllPluginProvider(string pluginsDirectory)
        {
            m_PluginsDirectory = pluginsDirectory;
        }

        public override async Task<ICollection<Assembly>> LoadPluginAssembliesAsync()
        {
            var assemblyList = new List<Assembly>();
            foreach (var file in Directory.GetFiles(m_PluginsDirectory, "*.dll"))
            {
                using (FileStream stream = File.Open(file, FileMode.Open))
                {
                    byte[] data = new byte[stream.Length];
                    await stream.ReadAsync(data, 0, (int)stream.Length);
                    var pluginAssembly = Assembly.Load(data);
                    assemblyList.Add(pluginAssembly);
                }
            }

            return assemblyList;
        }
    }
}