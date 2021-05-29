using System.Threading.Tasks;
using Semver;

namespace OpenMod.API.Plugins
{
    /// <summary>
    /// Represents an OpenMod plugin.
    /// </summary>
    public interface IOpenModPlugin : IOpenModComponent
    {
        /// <summary>
        /// Gets the human readable name of the plugin.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Gets the plugin author.
        /// </summary>
        string? Author { get; }

        /// <summary>
        /// Gets the plugin website.
        /// </summary>
        string? Website { get; }

        /// <summary>
        /// Gets the plugin description
        /// </summary>
        string? Description { get; }

        /// <summary>
        /// Gets the plugin version.
        /// </summary>
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