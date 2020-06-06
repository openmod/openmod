using System;
using System.Collections.Generic;

namespace OpenMod.Core.Plugins
{
    public class PluginManager
    {
        
    }

    public class PluginManagerOptions
    {
        public ICollection<Type> PluginProviders { get; } = new List<Type>();
    }
}