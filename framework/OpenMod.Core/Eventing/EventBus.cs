using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using OpenMod.Core.Helpers.Prioritization;
using OpenMod.Core.Prioritization;

namespace OpenMod.Core.Eventing
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class EventBus : IEventBus, IDisposable
    {
        private readonly ILogger<EventBus> m_Logger;
        private readonly List<EventSubscription> m_EventSubscriptions;

        public EventBus(ILogger<EventBus> logger)
        {
            m_Logger = logger;
            m_EventSubscriptions = new List<EventSubscription>();
        }

        public void Subscribe(IOpenModComponent component, string eventName, EventCallback callback)
        {
            if (!component.IsComponentAlive)
                return;

            var attribute = GetEventListenerAttribute(callback.Method);
            m_EventSubscriptions.Add(new EventSubscription(component, callback.Invoke, attribute, eventName));
        }

        public void Subscribe<TEvent>(IOpenModComponent component, EventCallback<TEvent> callback) where TEvent : IEvent
        {
            if (!component.IsComponentAlive)
                return;

            var attribute = GetEventListenerAttribute(callback.Method);
            m_EventSubscriptions.Add(new EventSubscription(component, (sender, @event) => callback.Invoke(sender, (TEvent)@event), attribute, typeof(TEvent)));
        }

        public void Subscribe(IOpenModComponent component, Type eventType, EventCallback callback)
        {
            if (!component.IsComponentAlive)
                return;

            var attribute = GetEventListenerAttribute(callback.Method);
            m_EventSubscriptions.Add(new EventSubscription(component, callback.Invoke, attribute, eventType));
        }

        public void Unsubscribe(IOpenModComponent component)
        {
            if (!component.IsComponentAlive)
                return;

            m_EventSubscriptions.RemoveAll(c => !c.Owner.IsAlive || c.Owner.Target == component);
        }

        public void Unsubscribe(IOpenModComponent component, string eventName)
        {
            if (!component.IsComponentAlive)
                return;

            m_EventSubscriptions.RemoveAll(c => (!c.Owner.IsAlive || c.Owner.Target == component) && c.EventName.Equals(eventName, StringComparison.OrdinalIgnoreCase));
        }

        public void Unsubscribe<TEvent>(IOpenModComponent component) where TEvent : IEvent
        {
            if (!component.IsComponentAlive)
                return;

            m_EventSubscriptions.RemoveAll(c => (!c.Owner.IsAlive || c.Owner.Target == component) && (c.EventType == typeof(TEvent) || c.EventName.Equals(typeof(TEvent).Name, StringComparison.OrdinalIgnoreCase)));
        }

        public void Unsubscribe(IOpenModComponent component, Type eventType)
        {
            if (!component.IsComponentAlive)
                return;

            m_EventSubscriptions.RemoveAll(c => (!c.Owner.IsAlive || c.Owner.Target == component) && (c.EventType == eventType || c.EventName.Equals(eventType.Name, StringComparison.OrdinalIgnoreCase)));
        }

        public void AddEventListener<TEvent>(IOpenModComponent component, IEventListener<TEvent> eventListener) where TEvent : IEvent
        {
            if (!component.IsComponentAlive)
                return;

            if (m_EventSubscriptions.Any(c => c.Listener?.GetType() == eventListener.GetType()))
            {
                // Prevent duplicate registration
                return;
            }

            var type = eventListener.GetType();

            foreach (var @interface in type.GetInterfaces().Where(c => typeof(IEventListener).IsAssignableFrom(c) && c.GetGenericArguments().Length == 1))
            {
                foreach (var method in @interface.GetMethods())
                {
                    var handler = GetEventListenerAttribute(method);
                    var eventType = @interface.GetGenericArguments()[0];

                    m_EventSubscriptions.Add(new EventSubscription(component, eventListener, method, handler, eventType));
                }
            }
        }

        public void RemoveEventListener<TEvent>(IEventListener<TEvent> eventListener) where TEvent : IEvent
        {
            m_EventSubscriptions.RemoveAll(c => c.Listener == eventListener);
        }

        public async Task EmitAsync(object sender, IEvent @event, EventExecutedCallback callback = null)
        {
            m_Logger.LogTrace($"Emitting event: {@event.Name}");
            var eventSubscriptions 
                = m_EventSubscriptions
                    .Where(c => (c.EventType != null && c.EventType.IsInstanceOfType(@event)) 
                                || (@event.Name.Equals(c.EventName, StringComparison.OrdinalIgnoreCase) 
                                   && c.Owner.IsAlive && ((IOpenModComponent)c.Owner.Target).IsComponentAlive))
                    .ToList();

        
            void Complete()
            {
                m_Logger.LogTrace($"{@event.Name}: Finished.");
                callback?.Invoke(@event);
            }

            if (eventSubscriptions.Count == 0)
            {
                m_Logger?.LogTrace($"{@event.Name}: No listeners found.");
                Complete();
                return;
            }

            var comparer = new PriorityComparer(PriortyComparisonMode.LowestFirst);
            eventSubscriptions.Sort((a, b) => comparer.Compare(a.EventListenerAttribute.Priority, b.EventListenerAttribute.Priority));

            foreach (var subscription in eventSubscriptions)
            {
                var owner = subscription.Owner;
                if (!owner.IsAlive || !((IOpenModComponent)owner.Target).IsComponentAlive)
                {
                    m_EventSubscriptions.Remove(subscription);
                    continue;
                }

                if (@event is ICancellableEvent cancellableEvent 
                    && cancellableEvent.IsCancelled 
                    && !subscription.EventListenerAttribute.IgnoreCancelled)
                {
                    continue;
                }

                await subscription.Callback.Invoke(sender, @event);
            }

            Complete();
        }

        private EventListenerAttribute GetEventListenerAttribute(MethodInfo method)
        {
            return method.GetCustomAttribute<EventListenerAttribute>() ?? new EventListenerAttribute();
        }

        public void Dispose()
        {
            m_EventSubscriptions.Clear();
        }
    }
}
