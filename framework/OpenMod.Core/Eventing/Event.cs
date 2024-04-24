using System.Collections.Generic;
using System.Reflection;

namespace OpenMod.Core.Eventing
{
    public abstract class Event : EventBase
    {
        public override Dictionary<string, object> Arguments
        {
            get
            {
                var args = new Dictionary<string, object>();
                var props = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var prop in props)
                {
                    var getter = prop.GetGetMethod(false);
                    if (getter == null)
                    {
                        continue;
                    }

                    args.Add(prop.Name.ToLower(), this);
                }

                return args;
            }
        }
    }
}