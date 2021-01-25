using System;
using System.Linq;
using JetBrains.Annotations;

namespace OpenMod.API.Plugins
{
    /// <summary>
    /// Provides metadata for plugins. Assemblies which do not have this attribute will not be loaded as plugins.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class PluginMetadataAttribute : Attribute
    {
        private string m_Id;

        public PluginMetadataAttribute(string id)
        {
            Id = id;
        }

        /// <value>
        /// The plugin ID. Cannot be null or empty. Must start with a letter and can only contain alphanumeric characters including dots.
        /// </value>
        public string Id
        {
            get { return m_Id; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception($"Invalid plugin ID: \"{value}\". Plugin IDs cannot be null or empty.");
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

        /// <value>
        /// The human-readable name of the plugin.
        /// </value>
        [CanBeNull]
        public string DisplayName { get; set; }

        /// <summary>
        /// The author of the plugin. Can be null.
        /// </summary>
        [CanBeNull]
        public string Author { get; set; }

        /// <summary>
        /// The website of the plugin. Can be null.
        /// </summary>
        [CanBeNull]
        public string Website { get; set; }
    }
}