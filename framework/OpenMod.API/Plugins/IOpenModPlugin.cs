using System.Threading.Tasks;
using JetBrains.Annotations;
using Semver;

namespace OpenMod.API.Plugins
{
    /// <summary>
    /// Represents an OpenMod plugin.
    /// </summary>
    public interface IOpenModPlugin : IOpenModComponent
    {
        /// <value>
        /// The human readable name of the plugin. Must not be null or empty.
        /// </value>
        [NotNull]
        string DisplayName { get; }

        /// <value>
        /// The plugin author. Can be null.
        /// </value>
        [CanBeNull]
        string Author { get; }

        /// <value>
        /// The plugin website. Can be null.
        /// </value>
        [CanBeNull]
        string Website { get; }

        /// <value>
        /// The plugin version. Must not be null.
        /// </value>
        [NotNull]
        SemVersion Version { get; }

        /// <summary>
        /// Loads the plugin.
        /// </summary>
        /// <remarks>
        /// <b>This method is for internal usage only and should not be used by plugins.</b>
        /// </remarks>
        [OpenModInternal]
        Task LoadAsync();

        /// <summary>
        /// Unloads the plugin.
        /// </summary>
        /// <remarks>
        /// <b>This method is for internal usage only and should not be used by plugins.</b>
        /// </remarks>
        [OpenModInternal]
        Task UnloadAsync();
    }
}