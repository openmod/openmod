namespace OpenMod.API.Eventing
{
    /// <summary>
    ///     Defines an event that can be cancelled.
    /// </summary>
    public interface ICancellableEvent : IEvent
    {
        /// <summary>
        ///     Sets if the event action should be cancelled.
        /// </summary>
        bool IsCancelled { get; set; }
    }
}