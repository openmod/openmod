using System;
using System.Linq;
using Semver;

namespace OpenMod.Core.Plugins
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public sealed class PluginMetadataAttribute : Attribute
    {
        private string m_Id;

        public string Id
        {
            get { return m_Id; }
            set
            {
                if (!value.All(d => char.IsLetterOrDigit(d) && d == '.'))
                {
                    throw new Exception($"Invalid plugin ID: \"{value}\". Plugin IDs can only consists of alphanumeric characters including dots.");
                }

                m_Id = value;
            }
        }

        public string DisplayName { get; set; }
     
        public string Author { get; set; }

        public SemVersion Version { get; set; }
    }
}