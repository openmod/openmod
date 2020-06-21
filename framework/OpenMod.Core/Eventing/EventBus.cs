using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using OpenMod.Core.Helpers;
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

        public virtual void Subscribe(IOpenModComponent component, string eventName, EventCallback callback)
        {
            if (!component.IsComponentAlive)
                return;

            var attribute = GetEventListenerAttribute(callback.Method);
            m_EventSubscriptions.Add(new EventSubscription(component, callback.Invoke, attribute, eventName, component.LifetimeScope.BeginLifetimeScope()));
        }

        public virtual void Subscribe<TEvent>(IOpenModComponent component, EventCallback<TEvent> callback) where TEvent : IEvent
        {
            if (!component.IsComponentAlive)
                return;

            var attribute = GetEventListenerAttribute(callback.Method);
            m_EventSubscriptions.Add(new EventSubscription(component, (serviceProvider, sender, @event) => callback.Invoke(serviceProvider, sender, (TEvent)@event), attribute, typeof(TEvent), component.LifetimeScope.BeginLifetimeScope()));
        }

        public virtual void Subscribe(IOpenModComponent component, Type eventType, EventCallback callback)
        {
            if (!component.IsComponentAlive)
                return;

            var attribute = GetEventListenerAttribute(callback.Method);
            m_EventSubscriptions.Add(new EventSubscription(component, callback.Invoke, attribute, eventType, component.LifetimeScope.BeginLifetimeScope()));
        }

        public virtual void Subscribe(IOpenModComponent component, Assembly assembly)
        {
            if (!component.IsComponentAlive)
            {
                return;
            }

            List<(Type eventListenerType, MethodInfo method, EventListenerAttribute eventListenerAttribute, Type eventType)> eventListeners = new List<(Type, MethodInfo, EventListenerAttribute, Type)>();
            var scope = component.LifetimeScope.BeginLifetimeScope((builder =>
            {
                foreach (var type in assembly.FindTypes<IEventListener>(false))
                {
                    if (m_EventSubscriptions.Any(c => c.EventListener == type))
                    {
                        // Prevent duplicate registration
                        return;
                    }

                    // ReSharper disable once LoopCanBeConvertedToQuery
                    foreach (var @interface in type.GetInterfaces().Where(c => typeof(IEventListener<>).IsAssignableFrom(c) && c.GetGenericArguments().Length >= 1))
                    {
                        var method = @interface.GetMethods().Single();
                        var handler = GetEventListenerAttribute(method);
                        var eventType = @interface.GetGenericArguments()[0];
                        eventListeners.Add((type, method, handler, eventType));
                    }

                    var lifetime = type.GetCustomAttribute<EventListenerLifetimeAttribute>()?.Lifetime ?? ServiceLifetime.Transient;

                    var registrationBuilder = builder.RegisterType(type)
                        .As(type)
                        .OwnedByLifetimeScope();

                    switch (lifetime)
                    {
                        case ServiceLifetime.Singleton:
                            registrationBuilder.SingleInstance();
                            break;
                        case ServiceLifetime.Scoped:
                            registrationBuilder.InstancePerLifetimeScope();
                            break;
                        case ServiceLifetime.Transient:
                            registrationBuilder.InstancePerRequest();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }));

            foreach (var eventListener in eventListeners)
            {
                m_EventSubscriptions.Add(new EventSubscription(component, eventListener.eventListenerType, eventListener.method, eventListener.eventListenerAttribute, eventListener.eventType, scope));
            }
        }

        public virtual void Unsubscribe(IOpenModComponent component)
        {
            if (!component.IsComponentAlive)
                return;

            m_EventSubscriptions.RemoveAll(c => !c.Owner.IsAlive || c.Owner.Target == component);
        }

        public virtual void Unsubscribe(IOpenModComponent component, string eventName)
        {
            if (!component.IsComponentAlive)
                return;

            m_EventSubscriptions.RemoveAll(c => (!c.Owner.IsAlive || c.Owner.Target == component) && c.EventName.Equals(eventName, StringComparison.OrdinalIgnoreCase));
        }

        public virtual void Unsubscribe<TEvent>(IOpenModComponent component) where TEvent : IEvent
        {
            if (!component.IsComponentAlive)
                return;

            m_EventSubscriptions.RemoveAll(c => (!c.Owner.IsAlive || c.Owner.Target == component) && (c.EventType == typeof(TEvent) || c.EventName.Equals(typeof(TEvent).Name, StringComparison.OrdinalIgnoreCase)));
        }

        public virtual void Unsubscribe(IOpenModComponent component, Type eventType)
        {
            if (!component.IsComponentAlive)
                return;

            m_EventSubscriptions.RemoveAll(c => (!c.Owner.IsAlive || c.Owner.Target == component) && (c.EventType == eventType || c.EventName.Equals(eventType.Name, StringComparison.OrdinalIgnoreCase)));
        }

        public virtual async Task EmitAsync(IOpenModComponent component, object sender, IEvent @event, EventExecutedCallback callback = null)
        {
            if (!component.IsComponentAlive)
            {
                return;
            }

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

                var serviceProvider = subscription.Scope.Resolve<IServiceProvider>();
                await subscription.Callback.Invoke(serviceProvider, sender, @event);
            }

            Complete();
        }

        protected virtual EventListenerAttribute GetEventListenerAttribute(MethodInfo method)
        {
            return method.GetCustomAttribute<EventListenerAttribute>() ?? new EventListenerAttribute();
        }

        public virtual void Dispose()
        {
            m_EventSubscriptions.Clear();
        }
    }
}
