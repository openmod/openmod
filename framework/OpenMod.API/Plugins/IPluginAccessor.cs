namespace OpenMod.API.Plugins
{
    /// <summary>
    /// Service used for resolving plugin instances. Must be lazy accessed on global scope services. 
    /// </summary>
    /// <typeparam name="TPlugin">The plugin to resolve.</typeparam>
    public interface IPluginAccessor<out TPlugin> where TPlugin : IOpenModPlugin
    {
        /// <summary>
        /// Gets the plugin instance. Returns null if the plugin is not loaded or found.
        /// </summary>
        TPlugin? Instance { get; }
    }
}