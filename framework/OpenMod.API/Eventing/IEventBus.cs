using System;
using System.Reflection;
using System.Threading.Tasks;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;

namespace OpenMod.API.Eventing
{
    /// <summary>
    ///     The type safe callback for event notifications.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <param name="sender">The event emitter.</param>
    /// <param name="event">The event instance.</param>
    public delegate Task EventCallback<in TEvent>(IServiceProvider serviceProvider, object sender, TEvent @event) where TEvent : IEvent;

    /// <summary>
    ///     The callback for event notifications.
    /// </summary>
    /// <param name="sender">The event emitter.</param>
    /// <param name="event">The event instance.</param>
    public delegate Task EventCallback(IServiceProvider serviceProvider, object sender, IEvent @event);

    /// <summary>
    ///     The emit callback for events that have finished and notified all listeners.
    /// </summary>
    /// <param name="event"></param>
    public delegate Task EventExecutedCallback(IEvent @event);

    /// <summary>
    ///     The event manager is responsible for emitting events and for managing their subscriptions.
    /// </summary>
    [Service]
    public interface IEventBus
    {
        /// <summary>
        ///     Subscribe to an event.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <param name="eventName">The event to subscribe to.</param>
        /// <param name="callback">The action to execute. See <see cref="EventCallback" /></param>
        void Subscribe(IOpenModComponent component, string eventName, EventCallback callback);

        /// <summary>
        ///     <inheritdoc cref="Subscribe(IOpenModComponent,string,EventCallback)" />
        /// </summary>
        /// <param name="component">The component.</param>
        /// <param name="callback">The action to execute after all listeners were notified.</param>
        /// <typeparam name="TEvent">The event to subscribe to.</typeparam>
        void Subscribe<TEvent>(IOpenModComponent component, EventCallback<TEvent> callback)
            where TEvent : IEvent;

        /// <summary>
        ///     <inheritdoc cref="Subscribe(IOpenModComponent,string,EventCallback)" />
        /// </summary>
        /// <param name="component">The component.</param>
        /// <param name="callback">The action to execute after all listeners were notified.</param>
        /// <param name="eventType">The event to subscribe to.</param>
        void Subscribe(IOpenModComponent component, Type eventType, EventCallback callback);

        /// <summary>
        ///    Automatically finds and registers IEventListeners for the component
        /// </summary>
        void Subscribe(IOpenModComponent component, Assembly assembly);

        /// <summary>
        ///     Unsubscribe all listener subscriptions of the given component.
        /// </summary>
        /// <param name="component">The component.</param>
        void Unsubscribe(IOpenModComponent component);

        /// <summary>
        ///     Unsubscribe all subscriptions for the given event type of the given component.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <param name="eventName">The event to unsubscribe from.</param>
        void Unsubscribe(IOpenModComponent component, string eventName);

        /// <summary>
        ///     Unsubscribe the event from this component type-safe
        /// </summary>
        /// <param name="component">The component.</param>
        void Unsubscribe<TEvent>(IOpenModComponent component) where TEvent : IEvent;

        /// <summary>
        ///     Unsubscribe all subscriptions for the given event type of the given component.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <param name="eventType">The event to unsubscribe from.</param>
        void Unsubscribe(IOpenModComponent component, Type eventType);

        /// <summary>
        ///     Emits an event and optionally handles the result
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="event">The event instance.</param>
        /// <param name="callback">The event finish callback. See <see cref="EventExecutedCallback" />.</param>
        Task EmitAsync(IOpenModComponent component, object sender, IEvent @event, EventExecutedCallback callback = null);
    }
}