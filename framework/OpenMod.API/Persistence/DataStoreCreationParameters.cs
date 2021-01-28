using System;

namespace OpenMod.API.Persistence
{
    /// <summary>
    /// Parameters for creating a new data store.
    /// </summary>
    public class DataStoreCreationParameters
    {
        /// <summary>
        /// Gets or sets the owning component id. Must not be null if <see cref="Component"/> is not set.
        /// </summary>
        /// <remarks>
        /// <b>Obsolete:</b> Use <see cref="Component"/> instead.
        /// </remarks>
        [Obsolete("Use the Component property instead.")]
        public string? ComponentId { get; set; }

        /// <summary>
        /// Gets or sets the owning component. Must not be null if <see cref="ComponentId"/> is not set.
        /// </summary>
        public IOpenModComponent? Component { get; set; }

        /// <summary>
        /// Gets or sets the working directory.
        /// </summary>
        public string WorkingDirectory { get; set; } = null!;

        /// <summary>
        /// Gets or sets the prefix.
        /// </summary>
        public string? Prefix { get; set; }

        /// <summary>
        /// Gets or sets the suffix.
        /// </summary>
        public string? Suffix { get; set; }

        /// <summary>
        /// Defines if detected file changes should be logged.
        /// </summary>
        public bool LogOnChange { get; set; } = true;
    }
}