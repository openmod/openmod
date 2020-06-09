using System.Collections.Generic;

namespace OpenMod.Core.Eventing
{
    /// <summary>
    ///    A basic event that does not need a separate class
    /// </summary>
    public sealed class BasicEvent : EventBase
    {
        public BasicEvent(string name) : base(name)
        {
            
        }

        public override Dictionary<string, object> Arguments { get; } = new Dictionary<string, object>();
    }
}