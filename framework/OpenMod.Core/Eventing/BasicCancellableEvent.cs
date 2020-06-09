using System.Collections.Generic;
using OpenMod.API.Eventing;

namespace OpenMod.Core.Eventing
{
    /// <summary>
    ///    A basic cancellable event that does not need a separate class
    /// </summary>
    public sealed class BasicCancellableEvent : EventBase, ICancellableEvent
    {
        public BasicCancellableEvent(string name) : base(name)
        {

        }

        public override Dictionary<string, object> Arguments { get; } = new Dictionary<string, object>();
        
        public bool IsCancelled { get; set; }
    }
}