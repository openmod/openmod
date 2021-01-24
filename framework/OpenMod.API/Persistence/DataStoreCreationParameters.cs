using System;
using JetBrains.Annotations;

namespace OpenMod.API.Persistence
{
    public class DataStoreCreationParameters
    {
        [Obsolete("Use the Component property instead.")]
        public string ComponentId { get; set; }

        [CanBeNull]
        public IOpenModComponent Component { get; set; }

        public string WorkingDirectory { get; set; }

        public string Prefix { get; set; }

        public string Suffix { get; set; }

        public bool LogOnChange { get; set; } = true;
    }
}