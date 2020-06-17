using System;
using System.Reflection;
using Autofac;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.Core.Helpers;

namespace OpenMod.Core.Eventing
{
    internal class EventSubscription
    {
        public EventSubscription(
            IOpenModComponent ownerComponent,
            EventCallback callback,
            EventListenerAttribute attribute,
            string eventName,
            ILifetimeScope scope)
        {
            Owner = new WeakReference(ownerComponent);
            Callback = callback;
            EventListenerAttribute = attribute;
            EventName = eventName;
            Scope = scope;
        }

        public EventSubscription(
            IOpenModComponent ownerComponent,
            Type eventListener,
            MethodInfo method,
            EventListenerAttribute attribute, 
            Type eventType,
            ILifetimeScope scope)
        {
            Owner = new WeakReference(ownerComponent);
            EventListener = eventListener;
            Callback = (serviceProvider, sender, @event) =>
            {
                var listener = serviceProvider.GetRequiredService(eventListener);
                return method.InvokeWithTaskSupportAsync(listener, new[] {sender, @event});
            };
            EventListenerAttribute = attribute;
            EventName = eventType.Name;
            EventType = eventType;
            Scope = scope;
        }

        public EventSubscription(
            IOpenModComponent ownerComponent,
            EventCallback callback,
            EventListenerAttribute attribute, 
            Type eventType,
            ILifetimeScope scope)
        {
            Owner = new WeakReference(ownerComponent);
            Callback = callback;
            EventListenerAttribute = attribute;
            EventName = eventType.Name;
            EventType = eventType;
            Scope = scope;
        }

        public string EventName { get; }

        public ILifetimeScope Scope { get; }

        [CanBeNull]
        public Type EventType { get; set; }

        public WeakReference Owner { get; set; }

        public EventCallback Callback { get; }

        public EventListenerAttribute EventListenerAttribute { get; }

        [CanBeNull]
        public Type EventListener { get; }
    }
}