namespace OpenMod.Core.Plugins
{
    /// <summary>
    /// The loading status of an OpenMod plugin.
    /// </summary>
    public enum PluginStatus
    {
        /// <summary>
        /// The plugin instance has been created but hasn't been loaded yet.
        /// </summary>
        Initialized,

        /// <summary>
        /// The plugin is loading, but has not yet finished.
        /// </summary>
        Loading,

        /// <summary>
        /// The plugin has finished loading.
        /// </summary>
        Loaded,

        /// <summary>
        /// The plugin is unloading, but has not yet finished.
        /// </summary>
        Unloading,

        /// <summary>
        /// The plugin has finished unloading.
        /// </summary>
        Unloaded,

        /// <summary>
        /// The plugin threw an exception during the loading process.
        /// </summary>
        ExceptionWhenLoading,

        /// <summary>
        /// The plugin threw an exception during the unloading process.
        /// </summary>
        ExceptionWhenUnloading
    }
}
