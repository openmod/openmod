using System;
using System.Linq;
using JetBrains.Annotations;

namespace OpenMod.API.Plugins
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class PluginMetadataAttribute : Attribute
    {
        private string m_Id;

        public PluginMetadataAttribute(string id)
        {
            Id = id;
        }

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

        public string DisplayName { get; set; }

        [CanBeNull]
        public string Author { get; set; }

        [CanBeNull]
        public string Website { get; set; }
    }
}