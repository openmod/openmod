namespace OpenMod.API
{
    /// <summary>
    ///     Defines an OpenMod component
    /// </summary>
    public interface IOpenModComponent
    {
        string OpenModComponentId { get; }
        bool IsComponentAlive { get; }
    }
}