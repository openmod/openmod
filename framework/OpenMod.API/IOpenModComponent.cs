using Autofac;

namespace OpenMod.API
{
    /// <summary>
    ///     Defines an OpenMod component
    /// </summary>
    public interface IOpenModComponent
    {
        string OpenModComponentId { get; }
 
        string WorkingDirectory { get; }
        
        bool IsComponentAlive { get; }

        ILifetimeScope LifetimeScope { get; }
    }
}