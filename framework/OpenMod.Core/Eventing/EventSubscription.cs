using Autofac;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.Common.Helpers;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace OpenMod.Core.Eventing
{
    [OpenModInternal]
    internal class EventSubscription : IAsyncDisposable
    {
        public EventSubscription(
            IOpenModComponent ownerComponent,
            EventCallback callback,
            IEventListenerOptions options,
            string eventName,
            ILifetimeScope scope)
        {
            Owner = new WeakReference(ownerComponent);
            Callback = callback;
            EventListenerOptions = options;
            EventName = eventName;
            Scope = scope;
        }

        public EventSubscription(
            IOpenModComponent ownerComponent,
            Type eventListener,
            MethodBase method,
            IEventListenerOptions options,
            Type eventType,
            ILifetimeScope scope)
        {
            Owner = new WeakReference(ownerComponent);
            EventListener = eventListener;
            Callback = (serviceProvider, sender, @event) =>
            {
                var listener = serviceProvider.GetRequiredService(eventListener);
                return method.InvokeWithTaskSupportAsync(listener, new[] { sender, @event });
            };
            EventListenerOptions = options;
            EventName = eventType.Name;
            EventType = eventType;
            Scope = scope;
        }

        public EventSubscription(
            IOpenModComponent ownerComponent,
            EventCallback callback,
            IEventListenerOptions options,
            Type eventType,
            ILifetimeScope scope)
        {
            Owner = new WeakReference(ownerComponent);
            Callback = callback;
            EventListenerOptions = options;
            EventName = eventType.Name;
            EventType = eventType;
            Scope = scope;
        }

        public string EventName { get; }

        public ILifetimeScope Scope { get; }

        public Type? EventType { get; set; }

        public WeakReference Owner { get; set; }

        public EventCallback Callback { get; }

        public IEventListenerOptions EventListenerOptions { get; }

        public Type? EventListener { get; }

        public ValueTask DisposeAsync()
        {
            return Scope.DisposeAsync();
        }
    }
}