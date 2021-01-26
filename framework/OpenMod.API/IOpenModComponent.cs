using Autofac;
using OpenMod.API.Persistence;

namespace OpenMod.API
{
    /// <summary>
    /// Defines an OpenMod component. Components are either plugins or OpenMod assemblies.
    /// </summary>
    public interface IOpenModComponent
    {
        /// <value>
        /// The component ID.
        /// </value>
        string OpenModComponentId { get; }

        /// <value>
        /// The working directory.
        /// </value>
        string WorkingDirectory { get; }

        /// <value>
        /// Checks if the component is alive. The component must not be able to execute any actions if its not alive.
        /// </value>
        bool IsComponentAlive { get; }

        /// <value>
        /// The components lifetime scope.
        /// </value>
        ILifetimeScope LifetimeScope { get; }

        /// <value>
        /// The components data store.
        /// </value>
        IDataStore? DataStore { get; }
    }
}