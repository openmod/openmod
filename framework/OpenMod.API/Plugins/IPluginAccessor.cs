namespace OpenMod.API.Plugins
{
    /// <summary>
    /// Service used for resolving plugin instances. Must be lazy accessed on global scope services. 
    /// </summary>
    /// <typeparam name="TPlugin">The plugin to resolve.</typeparam>
    public interface IPluginAccessor<out TPlugin> where TPlugin : IOpenModPlugin
    {
        /// <value>
        /// The plugin instance. Can be null if the plugin is not loaded or found.
        /// </value>
        TPlugin? Instance { get; }
    }
}