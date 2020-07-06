using System;
using JetBrains.Annotations;

namespace OpenMod.API.Ioc
{
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Class)]
    public class PluginServiceImplementationAttribute : ServiceImplementationAttribute
    {
        
    }
}