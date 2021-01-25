using Autofac;
using JetBrains.Annotations;
using OpenMod.API.Persistence;

namespace OpenMod.API
{
    /// <summary>
    /// Defines an OpenMod component. Components are either plugins or OpenMod assemblies.
    /// </summary>
    public interface IOpenModComponent
    {
        /// <value>
        /// The component ID. Cannot be null or empty.
        /// </value>
        [NotNull]
        string OpenModComponentId { get; }

        /// <value>
        /// The working directory. Cannot be null or empty.
        /// </value>
        [NotNull]
        string WorkingDirectory { get; }

        /// <value>
        /// Checks if the component is alive. The component must not be able to execute any actions if its not alive.
        /// </value>
        bool IsComponentAlive { get; }

        /// <value>
        /// The components lifetime scope. Cannot be null.
        /// </value>
        [NotNull]
        ILifetimeScope LifetimeScope { get; }

        /// <value>
        /// The components data store. Can be null.
        /// </value>
        [CanBeNull]
        IDataStore DataStore { get; }
    }
}