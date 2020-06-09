using System.Threading.Tasks;

namespace OpenMod.API.Eventing
{
    /// <summary>
    ///     A listener for one or more events.
    /// </summary>
    public interface IEventListener { }
    /// <summary>
    ///     <inheritdoc />
    /// </summary>
    /// <typeparam name="TEvent">The event to subscribe to.</typeparam>
    public interface IEventListener<in TEvent> : IEventListener where TEvent : IEvent
    {
        /// <summary>
        ///     Called when an event got emitted.
        /// </summary>
        /// <param name="emitter">The event emitter.</param>
        /// <param name="event">The event instance.</param>
        Task HandleEventAsync(object emitter, TEvent @event);
    }
}