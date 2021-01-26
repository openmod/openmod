using System;

namespace OpenMod.API.Persistence
{
    /// <summary>
    /// Parameters for creating a new data store.
    /// </summary>
    public class DataStoreCreationParameters
    {
        /// <value>
        /// The owning component id. Must not be null if <see cref="Component"/> is not set.
        /// </value>
        /// <remarks>
        /// <b>Obsolete:</b> Use <see cref="Component"/> instead.
        /// </remarks>
        [Obsolete("Use the Component property instead.")]
        public string? ComponentId { get; set; }

        /// <value>
        /// The owning component. Must not be null if <see cref="ComponentId"/> is not set.
        /// </value>
        public IOpenModComponent? Component { get; set; }

        /// <value>
        /// The working directory.
        /// </value>
        public string WorkingDirectory { get; set; } = null!;

        /// <value>
        /// The prefix. Can be null or empty.
        /// </value>
        public string? Prefix { get; set; } 

        /// <value>
        /// The suffix. Can be null or empty.
        /// </value>
        public string? Suffix { get; set; }

        /// <value>
        /// <b>True</b> if changes should be logged.
        /// </value>
        public bool LogOnChange { get; set; } = true;
    }
}