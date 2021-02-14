using System.Threading.Tasks;

namespace OpenMod.API.Eventing
{
    /// <summary>
    /// Listens for an events.
    /// </summary>
    public interface IEventListener { }
    /// <summary>
    ///     <inheritdoc />
    /// </summary>
    /// <typeparam name="TEvent">The event to listen to.</typeparam>
    public interface IEventListener<in TEvent> : IEventListener where TEvent : IEvent
    {
        /// <summary>
        /// Called when the event has been emitted.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="event">The event object.</param>
        Task HandleEventAsync(object? sender, TEvent @event);
    }
}