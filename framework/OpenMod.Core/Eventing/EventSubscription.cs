using System;
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
            EventListenerData eventListenerData,
            ILifetimeScope scope)
        {
            Owner = new WeakReference(ownerComponent);
            EventListener = eventListenerData.EventListenerType;
            Callback = (serviceProvider, sender, @event) =>
            {
                var listener = serviceProvider.GetRequiredService(eventListenerData.EventListenerType);
                return eventListenerData.Method.InvokeWithTaskSupportAsync(listener, new[] {sender, @event});
            };
            EventListenerAttribute = eventListenerData.EventListenerAttribute;
            EventName = eventListenerData.EventListenerType.Name;
            EventType = eventListenerData.EventListenerType;
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