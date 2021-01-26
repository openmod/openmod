namespace OpenMod.API.Eventing
{
    /// <summary>
    /// Represents a cancellable event.
    /// </summary>
    public interface ICancellableEvent : IEvent
    {
        /// <summary>
        /// Gets or sets if the event action should be cancelled.
        /// </summary>
        bool IsCancelled { get; set; }
    }
}