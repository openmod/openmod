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
using OpenMod.Core.Ioc.Extensions;
using OpenMod.Core.Prioritization;

namespace OpenMod.Core.Eventing
{
    [OpenModInternal]
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class EventBus : IEventBus, IAsyncDisposable
    {
        private bool m_IsDisposing;

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
                    foreach (var @interface in type.GetInterfaces().Where(c => typeof(IEventListener).IsAssignableFrom(c) && c.GetGenericArguments().Length > 0))
                    {
                        var interfaceMethod = @interface.GetMethods().Single();
                        var map = type.GetInterfaceMap(interfaceMethod.DeclaringType);
                        var index = Array.IndexOf(map.InterfaceMethods, interfaceMethod);

                        var method = map.TargetMethods[index];
                        var handler = GetEventListenerAttribute(method);
                        var eventType = @interface.GetGenericArguments()[0];
                        eventListeners.Add((type, method, handler, eventType));
                    }

                    var lifetime = type.GetCustomAttribute<EventListenerLifetimeAttribute>()?.Lifetime ?? ServiceLifetime.Transient;

                    builder.RegisterType(type)
                        .As(type)
                        .WithLifetime(lifetime)
                        .OwnedByLifetimeScope();
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

        public virtual async Task EmitAsync(IOpenModComponent component, object sender, IEvent @event,
            EventExecutedCallback callback = null)
        {
            if (!component.IsComponentAlive)
            {
                return;
            }

            var eventTypes = new List<Type>();

            var currentType = @event.GetType();
            while (currentType != null && !currentType.IsAbstract && !currentType.IsInterface && typeof(IEvent).IsAssignableFrom(currentType))
            {
                eventTypes.Add(currentType);
                currentType = currentType.BaseType;
            }
            
            eventTypes.AddRange(@event.GetType().GetInterfaces().Where(d => typeof(IEvent).IsAssignableFrom(d)));

            foreach (var eventType in eventTypes)
            {
                string eventName = GetEventName(eventType);

                m_Logger.LogTrace($"Emitting event: {eventName}");
                var eventSubscriptions
                    = m_EventSubscriptions
                        .Where(c => (c.EventType != null && c.EventType == eventType)
                                    || (eventName.Equals(c.EventName, StringComparison.OrdinalIgnoreCase)
                                        && c.Owner.IsAlive && ((IOpenModComponent)c.Owner.Target).IsComponentAlive))
                        .ToList();


                if (eventSubscriptions.Count == 0)
                {
                    m_Logger?.LogTrace($"{eventName}: No listeners found.");
                    continue;
                }

                var comparer = new PriorityComparer(PriortyComparisonMode.LowestFirst);
                eventSubscriptions.Sort((a, b) =>
                    comparer.Compare(
                        (Priority)a.EventListenerAttribute.Priority,
                        (Priority)b.EventListenerAttribute.Priority)
                );

                foreach (var group in eventSubscriptions.GroupBy(e => e.Scope))
                {
                    await using var newScope = group.Key.BeginLifetimeScope("AutofacWebRequest");
                    foreach (var subscription in group)
                    {
                        var cancellableEvent = @event as ICancellableEvent;

                        if (cancellableEvent != null
                            && cancellableEvent.IsCancelled
                            && !subscription.EventListenerAttribute.IgnoreCancelled)
                        {
                            continue;
                        }

                        var wasCancelled = false;
                        if (cancellableEvent != null)
                        {
                            wasCancelled = cancellableEvent.IsCancelled;
                        }

                        var serviceProvider = newScope.Resolve<IServiceProvider>();

                        try
                        {
                            await subscription.Callback.Invoke(serviceProvider, sender, @event);

                            if (cancellableEvent != null && subscription.EventListenerAttribute.Priority ==
                                EventListenerPriority.Monitor)
                            {
                                if (cancellableEvent.IsCancelled != wasCancelled)
                                {
                                    cancellableEvent.IsCancelled = wasCancelled;
                                    m_Logger.LogWarning(
                                        $"{((IOpenModComponent)@subscription.Owner.Target).OpenModComponentId} changed {@eventName} cancellation status with Monitor priority which is not permitted.");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            m_Logger.LogError(ex, $"Exception occured during event {@eventName}");
                        }
                    }
                }

                m_Logger.LogTrace($"{eventName}: Finished.");
            }

            callback?.Invoke(@event);
        }

        internal static string GetEventName(Type eventType)
        {
            // remove "Event" suffix from type name

            const string suffix = "Event";
            var eventName = eventType.Name;
            var idx = eventName.IndexOf(suffix, StringComparison.Ordinal);

            return idx == -1
                ? eventName
                : eventName.Remove(idx, suffix.Length);
        }

        protected virtual EventListenerAttribute GetEventListenerAttribute(MethodInfo method)
        {
            return method.GetCustomAttribute<EventListenerAttribute>() ?? new EventListenerAttribute();
        }

        public virtual async ValueTask DisposeAsync()
        {
            if (m_IsDisposing)
            {
                return;
            }
            m_IsDisposing = true;

            await m_EventSubscriptions
                .Select(d => d.Scope)
                .Distinct()
                .DisposeAllAsync();

            m_EventSubscriptions.Clear();
        }
    }
}
