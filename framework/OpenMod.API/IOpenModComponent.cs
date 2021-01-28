using Autofac;
using OpenMod.API.Persistence;

namespace OpenMod.API
{
    /// <summary>
    /// Defines an OpenMod component. Components are either plugins or OpenMod assemblies.
    /// </summary>
    public interface IOpenModComponent
    {
        /// <summary>
        /// Gets the component ID.
        /// </summary>
        string OpenModComponentId { get; }

        /// <summary>
        /// Gets the working directory.
        /// </summary>
        string WorkingDirectory { get; }

        /// <summary>
        /// Checks if the component is alive. The component must not be able to execute any actions if this returns false.
        /// </summary>
        bool IsComponentAlive { get; }

        /// <summary>
        /// Gets the components lifetime scope.
        /// </summary>
        ILifetimeScope LifetimeScope { get; }

        /// <summary>
        /// Gets the optional data store of the component.
        /// </summary>
        IDataStore? DataStore { get; }
    }
}