using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.Core.Helpers;
using Oxide.Core.Plugins;

namespace OpenMod.Rust.Oxide.Events
{
    internal abstract class OxideEventsListenerBase : CSPlugin
    {
        private readonly IEventBus m_EventBus;
        private readonly IOpenModHost m_OpenModHost;

        protected OxideEventsListenerBase(IEventBus eventBus, IOpenModHost openModHost)
        {
            m_EventBus = eventBus;
            m_OpenModHost = openModHost;

            Title = "OpenModInternal_" + GetType().FullName;
            Author = "OpenMod Contributors";
        }

        protected void Emit(IEvent @event)
        {
            AsyncHelper.RunSync(() => m_EventBus.EmitAsync(m_OpenModHost, this, @event));
        }

        protected bool EmitCancellableReturnsBool(ICancellableEvent @event)
        {
            Emit(@event);
            return !@event.IsCancelled;
        }

        protected object? EmitCancellableReturnsObject(ICancellableEvent @event)
        {
            Emit(@event);
            return @event.IsCancelled ? new object() : null;
        }
    }
}
