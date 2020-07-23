using System;
using System.Threading.Tasks;

namespace OpenMod.API.Eventing
{
    /// <summary>
    ///     The type safe callback for event notifications.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <param name="sender">The event sender.</param>
    /// <param name="event">The event instance.</param>
    public delegate Task EventCallback<in TEvent>(IServiceProvider serviceProvider, object sender, TEvent @event) where TEvent : IEvent;
}