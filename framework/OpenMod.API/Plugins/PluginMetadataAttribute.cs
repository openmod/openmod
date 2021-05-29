using System;
using System.Linq;

namespace OpenMod.API.Plugins
{
    /// <summary>
    /// Provides metadata for plugins. Assemblies which do not have this attribute will not be loaded as plugins.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class PluginMetadataAttribute : Attribute
    {
        private string? m_Id;

        public PluginMetadataAttribute(string id)
        {
            Id = id;
        }

        /// <summary>
        /// Gets or sets the plugin ID. Must start with a letter and can only contain alphanumeric characters including dots.
        /// </summary>
        public string Id
        {
            get { return m_Id!; }
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

                // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
                if (!value.All(d => char.IsLetterOrDigit(d) || d == '.'))
                {
                    throw new Exception($"Invalid plugin ID: \"{value}\". Plugin IDs can only consists of alphanumeric characters including dots.");
                }

                m_Id = value;
            }
        }

        /// <summary>
        /// Gets or sets the human-readable name of the plugin.
        /// </summary>
        public string? DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the author of the plugin.
        /// </summary>
        public string? Author { get; set; }

        /// <summary>
        /// Gets or sets the website of the plugin.
        /// </summary>
        public string? Website { get; set; }

        /// <summary>
        /// Gets or sets the description of the plugin.
        /// </summary>
        public string? Description { get; set; }
    }
}