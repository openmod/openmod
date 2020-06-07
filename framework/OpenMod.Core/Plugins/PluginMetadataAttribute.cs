using System;
using System.Linq;
using Semver;

namespace OpenMod.Core.Plugins
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public sealed class PluginMetadataAttribute : Attribute
    {
        private string m_Id;
        private string m_Version;

        public string Id
        {
            get { return m_Id; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception($"Invalid plugin ID: \"{value}\". Plugin IDs can not be empty.");
                }

                if (!char.IsLetter(value[0]))
                {
                    throw new Exception($"Invalid plugin ID: \"{value}\". Plugin IDs must start with a letter.");
                }

                if (!value.All(d => char.IsLetterOrDigit(d) || d == '.'))
                {
                    throw new Exception($"Invalid plugin ID: \"{value}\". Plugin IDs can only consists of alphanumeric characters including dots.");
                }

                m_Id = value;
            }
        }

        public string DisplayName { get; set; }
     
        public string Author { get; set; }

        public string Version
        {
            get { return m_Version; }
            set
            {
                if (!SemVersion.TryParse(value, out _, strict: false))
                {
                    throw new Exception($"Invalid semver \"{Version}\" in metadata for plugin: {Id}");
                }
                m_Version = value;
            }
        }
    }
}