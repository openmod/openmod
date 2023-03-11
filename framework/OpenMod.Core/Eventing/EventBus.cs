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
using OpenMod.Common.Helpers;
using OpenMod.Core.Helpers;
using OpenMod.Core.Ioc;
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
        private readonly List<EventSubscription> m_EventSubscriptions = new();
        private readonly object m_Lock = new();

        private static readonly Type[] s_OmittedTypes =
        {
            typeof(IEvent),
            typeof(ICancellableEvent),
            typeof(EventBase),
            typeof(Event)
        };

        private static readonly PriorityComparer s_PriorityComparer = new(PriortyComparisonMode.LowestFirst);

        public EventBus(ILogger<EventBus> logger)
        {
            m_Logger = logger;
        }

        private IDisposable SubscribeInternal(EventSubscription subscription)
        {
            subscription.Scope.Disposer.AddInstanceForDisposal(new DisposeAction(() =>
            {
                lock (m_Lock)
                {
                    m_EventSubscriptions.RemoveAll(x => x.Scope == subscription.Scope);
                }
            }));

            lock (m_Lock)
            {
                m_EventSubscriptions.Add(subscription);
            }

            return new DisposeAction(() =>
            {
                lock (m_Lock)
                {
                    m_EventSubscriptions.Remove(subscription);
                }
            });
        }

        public virtual IDisposable Subscribe(IOpenModComponent component, string eventName, EventCallback callback)
        {
            if (callback == null) throw new ArgumentNullException(nameof(callback));

            var attribute = GetEventListenerAttribute(callback.Method);

            return Subscribe(component, eventName, callback, attribute);
        }

        public virtual IDisposable Subscribe(IOpenModComponent component, string eventName, EventCallback callback,
            IEventListenerOptions options)
        {
            if (component == null) throw new ArgumentNullException(nameof(component));

            if (callback == null) throw new ArgumentNullException(nameof(callback));

            if (string.IsNullOrEmpty(eventName)) throw new ArgumentException(eventName);

            if (options == null) throw new ArgumentNullException(nameof(options));

            if (!component.IsComponentAlive)
            {
                m_Logger.LogDebug("{ComponentId} tried to subscribe a callback but the component is not alive",
                    component.OpenModComponentId);
                return NullDisposable.Instance;
            }

            var subscription = new EventSubscription(component, callback.Invoke, options, eventName,
                component.LifetimeScope.BeginLifetimeScopeEx());

            return SubscribeInternal(subscription);
        }

        public virtual IDisposable Subscribe<TEvent>(IOpenModComponent component, EventCallback<TEvent> callback)
            where TEvent : IEvent
        {
            if (callback == null) throw new ArgumentNullException(nameof(callback));

            var attribute = GetEventListenerAttribute(callback.Method);

            return Subscribe(component, callback, attribute);
        }

        public virtual IDisposable Subscribe<TEvent>(IOpenModComponent component, EventCallback<TEvent> callback,
            IEventListenerOptions options) where TEvent : IEvent
        {
            if (component == null) throw new ArgumentNullException(nameof(component));

            if (callback == null) throw new ArgumentNullException(nameof(callback));

            if (options == null) throw new ArgumentNullException(nameof(options));

            if (!component.IsComponentAlive)
            {
                m_Logger.LogDebug("{ComponentId} tried to subscribe a callback but the component is not alive",
                    component.OpenModComponentId);
                return NullDisposable.Instance;
            }

            var subscription = new EventSubscription(component,
                (serviceProvider, sender, @event) => callback.Invoke(serviceProvider, sender, (TEvent)@event),
                options, typeof(TEvent), component.LifetimeScope.BeginLifetimeScopeEx());

            return SubscribeInternal(subscription);
        }

        public virtual IDisposable Subscribe(IOpenModComponent component, Type eventType, EventCallback callback)
        {
            if (callback == null) throw new ArgumentNullException(nameof(callback));

            var attribute = GetEventListenerAttribute(callback.Method);

            return Subscribe(component, eventType, callback, attribute);
        }

        public virtual IDisposable Subscribe(IOpenModComponent component, Type eventType, EventCallback callback,
            IEventListenerOptions options)
        {
            if (component == null) throw new ArgumentNullException(nameof(component));

            if (callback == null) throw new ArgumentNullException(nameof(callback));

            if (options == null) throw new ArgumentNullException(nameof(options));

            if (!component.IsComponentAlive)
            {
                m_Logger.LogDebug("{ComponentId} tried to subscribe a callback but the component is not alive",
                    component.OpenModComponentId);
                return NullDisposable.Instance;
            }

            var subscription = new EventSubscription(component, callback.Invoke, options, eventType,
                component.LifetimeScope.BeginLifetimeScopeEx());

            return SubscribeInternal(subscription);
        }

        public virtual IDisposable Subscribe(IOpenModComponent component, Assembly assembly)
        {
            if (component == null) throw new ArgumentNullException(nameof(component));

            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            if (!component.IsComponentAlive)
            {
                m_Logger.LogDebug("{ComponentId} tried to subscribe \"{AsemblyName}\" but the component is not alive",
                    component.OpenModComponentId, assembly.FullName);
                return NullDisposable.Instance;
            }

            m_Logger.LogDebug("Subscribing assembly \"{AssemblyName}\" for {ComponentId}",
                assembly.FullName, component.OpenModComponentId);

            List<(Type eventListenerType, MethodInfo method, EventListenerAttribute eventListenerAttribute, Type
                eventType)> eventListeners = new();
            var scope = component.LifetimeScope.BeginLifetimeScopeEx(builder =>
            {
                foreach (var type in assembly.FindTypes<IEventListener>())
                {
                    lock (m_Lock)
                    {
                        if (m_EventSubscriptions.Any(c => c.EventListener == type))
                        {
                            // Prevent duplicate registration
                            return;
                        }
                    }

                    // ReSharper disable once LoopCanBeConvertedToQuery
                    foreach (var @interface in type.GetInterfaces().Where(c => typeof(IEventListener).IsAssignableFrom(c) && c.GetGenericArguments().Length > 0))
                    {
                        var interfaceMethod = @interface.GetMethods().Single();
                        if (interfaceMethod.DeclaringType == null)
                        {
                            continue;
                        }

                        var map = type.GetInterfaceMap(interfaceMethod.DeclaringType);
                        var index = Array.IndexOf(map.InterfaceMethods, interfaceMethod);

                        var method = map.TargetMethods[index];
                        var handler = GetEventListenerAttribute(method);
                        var eventType = @interface.GetGenericArguments()[0];
                        eventListeners.Add((type, method, handler, eventType));
                    }

                    var lifetime = type.GetCustomAttribute<EventListenerLifetimeAttribute>()?.Lifetime ??
                                   ServiceLifetime.Transient;

                    builder.RegisterType(type)
                        .As(type)
                        .WithLifetime(lifetime)
                        .OwnedByLifetimeScope();
                }
            });

            var eventDisposables = new List<IDisposable>(eventListeners.Count);

            foreach (var (eventListenerType, method, eventListenerAttribute, eventType) in eventListeners)
            {
                var subscription = new EventSubscription(component, eventListenerType,
                    method, eventListenerAttribute, eventType, scope);

                var disposable = SubscribeInternal(subscription);

                eventDisposables.Add(disposable);
            }

            return new DisposeAction(eventDisposables.DisposeAll);
        }

        public virtual void Unsubscribe(IOpenModComponent component)
        {
            if (component == null) throw new ArgumentNullException(nameof(component));

            lock (m_Lock)
            {
                m_EventSubscriptions.RemoveAll(c => !c.Owner.IsAlive || c.Owner.Target == component);
            }
        }

        public virtual void Unsubscribe(IOpenModComponent component, string eventName)
        {
            if (component == null) throw new ArgumentNullException(nameof(component));

            lock (m_Lock)
            {
                m_EventSubscriptions.RemoveAll(c =>
                    (!c.Owner.IsAlive || c.Owner.Target == component) &&
                    c.EventName.Equals(eventName, StringComparison.OrdinalIgnoreCase));
            }
        }

        public virtual void Unsubscribe<TEvent>(IOpenModComponent component) where TEvent : IEvent
        {
            if (component == null) throw new ArgumentNullException(nameof(component));

            lock (m_Lock)
            {
                m_EventSubscriptions.RemoveAll(c =>
                    (!c.Owner.IsAlive || c.Owner.Target == component) && (c.EventType == typeof(TEvent) ||
                                                                          c.EventName.Equals(typeof(TEvent).Name,
                                                                              StringComparison.OrdinalIgnoreCase)));
            }
        }

        public virtual void Unsubscribe(IOpenModComponent component, Type eventType)
        {
            if (component == null) throw new ArgumentNullException(nameof(component));

            lock (m_Lock)
            {
                m_EventSubscriptions.RemoveAll(c =>
                    (!c.Owner.IsAlive || c.Owner.Target == component) && (c.EventType == eventType ||
                                                                          c.EventName.Equals(eventType.Name,
                                                                              StringComparison.OrdinalIgnoreCase)));
            }
        }

        public virtual async Task EmitAsync(IOpenModComponent component, object? sender, IEvent @event,
            EventExecutedCallback? callback = null)
        {
            if (component == null) throw new ArgumentNullException(nameof(component));

            if (@event == null) throw new ArgumentNullException(nameof(@event));

            if (!component.IsComponentAlive)
            {
                m_Logger.LogDebug("EmitAsync called by {ComponentId} for {EventType} but the component is not alive",
                    component.OpenModComponentId, @event.GetType().Name);
                return;
            }

            var eventTypes = new List<Type>();
            var currentType = @event.GetType();

            while (currentType != null && typeof(IEvent).IsAssignableFrom(currentType))
            {
                eventTypes.Add(currentType);
                currentType = currentType.BaseType;
            }

            eventTypes.AddRange(@event.GetType().GetInterfaces().Where(d => typeof(IEvent).IsAssignableFrom(d)) );

            var cancellableEvent = @event as ICancellableEvent;

            foreach (var eventType in eventTypes.Except(s_OmittedTypes))
            {
                var eventName = GetEventName(eventType);

                m_Logger.LogTrace("Emitting event: {EventName}", eventName);
                List<EventSubscription> eventSubscriptions;

                lock (m_Lock)
                {
                    eventSubscriptions = m_EventSubscriptions
                        .Where(c => c is not null && ((c.EventType != null && c.EventType == eventType)
                                                      || (eventName.Equals(c.EventName,
                                                              StringComparison.OrdinalIgnoreCase)
                                                          && c.Owner.IsAlive &&
                                                          ((IOpenModComponent)c.Owner.Target).IsComponentAlive)))
                        .ToList();
                }

                if (eventSubscriptions.Count == 0)
                {
                    m_Logger.LogTrace("No event subscriptions found for: {EventName}", eventName);
                    continue;
                }

                eventSubscriptions.Sort((a, b) =>
                    s_PriorityComparer.Compare(
                        (Priority)a.EventListenerOptions.Priority,
                        (Priority)b.EventListenerOptions.Priority)
                );

                foreach (var group in eventSubscriptions.GroupBy(e => e.Scope))
                {
                    try
                    {
                        //   Creates a new scope for the event. This is needed for scoped services so they share the same service instance
                        // on each events. AutofacWebRequest makes it emulate a request for proper scopes. This tag is hardcoded by AutoFac.
                        // Without this tag, services with the "Scope" lifetime will cause "DependencyResolutionException:
                        // No scope with a Tag matching 'AutofacWebRequest' (...)".
                        //
                        //   If you are here because of the following error:  "System.ObjectDisposedException: Instances cannot
                        // be resolved and nested lifetimes cannot be created from this LifetimeScope as it (or one of its parent scopes)
                        // has already been disposed." It means you injected a service to an IEventHandler
                        // that used the service *after* the event has finished (e.g. in a Task or by storing it somewhere).
                        await using var newScope = group.Key.BeginLifetimeScope("AutofacWebRequest");
                        foreach (var subscription in group)
                        {
                            if (cancellableEvent is { IsCancelled: true } && !subscription.EventListenerOptions.IgnoreCancelled)
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

                                // Ensure monitor event listeners can't cancel or uncancel events
                                if (cancellableEvent != null && subscription.EventListenerOptions.Priority ==
                                    EventListenerPriority.Monitor)
                                {
                                    if (cancellableEvent.IsCancelled != wasCancelled)
                                    {
                                        cancellableEvent.IsCancelled = wasCancelled;
                                        m_Logger.LogWarning(
                                            "{ComponentId} changed {EventName} cancellation status with Monitor priority which is not permitted",
                                            ((IOpenModComponent)subscription.Owner.Target).OpenModComponentId,
                                            eventName);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                m_Logger.LogError(ex, "Exception occured during event {EventName}", eventName);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        m_Logger.LogError(ex, "Exception occurred when attempting to emit event {EventName}",
                            eventName);
                    }
                }

                m_Logger.LogTrace("{EventName}: Finished", eventName);
            }

            if (callback != null)
            {
                await callback.Invoke(@event);
            }
        }

        internal static string GetEventName(Type eventType)
        {
            // remove "Event" suffix from type name

            const string suffix = "Event";
            var eventName = eventType.Name;

            return eventName.EndsWith(suffix, StringComparison.Ordinal)
                ? eventName[..^suffix.Length]
                : eventName;
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

            List<ILifetimeScope> scopes;
            lock (m_Lock)
            {
                scopes = m_EventSubscriptions
                    .Select(d => d.Scope)
                    .Distinct()
                    .ToList();
                m_EventSubscriptions.Clear();
            }

            await scopes.DisposeAllAsync();
        }
    }
}