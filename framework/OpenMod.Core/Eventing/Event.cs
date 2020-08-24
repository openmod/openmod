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
                Dictionary<string, object> args = new Dictionary<string, object>();
                PropertyInfo[] props = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo prop in props)
                {
                    MethodInfo getter = prop.GetGetMethod(false);
                    if (getter == null) continue;

                    args.Add(prop.Name.ToLower(), this);
                }

                return args;
            }
        }
    }
}